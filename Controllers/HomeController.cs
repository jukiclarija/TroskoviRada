using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TroskoviRada.Data;
using TroskoviRada.Models;

namespace TroskoviRada.Controllers {
    public class HomeController : Controller {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index() {
            try {
                var statistika = new {
                    BrojZaposlenika = await _context.Zaposlenici.CountAsync(),
                    BrojRadnihMjesta = await _context.RadnaMjesta.CountAsync(),
                    BrojSmjena = await _context.Smjene.CountAsync(),
                    BrojNaknada = await _context.TipoviNaknada.CountAsync(),
                };

                ViewBag.Statistika = statistika;
                return View();
            } catch (Exception) {
                ViewBag.Error = "Greška pri učitavanju statistike";
                return View();
            }
        }

        // ZAPOSLENICI
        public async Task<IActionResult> Zaposlenici() {
            var zaposlenici = await _context.Zaposlenici
                .Include(z => z.Zaposlenja)
                    .ThenInclude(z => z.RadnoMjesto)
                .OrderByDescending(z => z.IdZaposlenik)
                .ToListAsync();
            return View(zaposlenici);
        }

        [HttpGet]
        public async Task<IActionResult> DodajZaposlenika() {
            try {
                var radnaMjesta = await _context.RadnaMjesta.OrderBy(rm => rm.Naziv).ToListAsync();
                var tipoviNaknada = await _context.TipoviNaknada.OrderBy(t => t.Naziv).ToListAsync();

                ViewBag.RadnaMjesta = radnaMjesta;
                ViewBag.TipoviNaknada = tipoviNaknada;

                return View();
            } catch (Exception) {
                ViewBag.RadnaMjesta = new List<RadnoMjesto>();
                ViewBag.TipoviNaknada = new List<TipNaknade>();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajZaposlenika(
            [Bind("Ime,Prezime,BrojRacuna,Email,Telefon,Oib,RadnoMjestoId,DatumPocetka,Naknade")]
            ZaposlenikViewModel model) {
            if (ModelState.IsValid) {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try {
                    var zaposlenik = new Zaposlenik {
                        Ime = model.Ime,
                        Prezime = model.Prezime,
                        BrojRacuna = model.BrojRacuna,
                        Email = model.Email,
                        Telefon = model.Telefon,
                        Oib = model.Oib
                    };

                    _context.Add(zaposlenik);
                    await _context.SaveChangesAsync();

                    if (model.RadnoMjestoId > 0) {
                        var zaposlenje = new Zaposlenje {
                            IdZaposlenik = zaposlenik.IdZaposlenik,
                            IdRadnoMjesto = model.RadnoMjestoId,
                            DatumOd = model.DatumPocetka,
                            DatumDo = null
                        };
                        _context.Add(zaposlenje);
                        await _context.SaveChangesAsync();
                    }

                    if (model.Naknade != null && model.Naknade.Any()) {
                        var validneNaknade = model.Naknade
                            .Where(n => n.TipNaknadeId > 0 && n.Datum != default)
                            .ToList();

                        foreach (var naknadaVM in validneNaknade) {
                            var naknada = new Naknada {
                                IdZaposlenik = zaposlenik.IdZaposlenik,
                                IdTipNaknade = naknadaVM.TipNaknadeId,
                                Datum = naknadaVM.Datum,
                                Iznos = 0
                            };
                            _context.Add(naknada);
                        }
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Zaposlenik uspješno dodan!";
                    return RedirectToAction(nameof(Zaposlenici));
                } catch (Exception ex) {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Greška pri spremanju: {ex.Message}");
                }
            }

            ViewBag.RadnaMjesta = await _context.RadnaMjesta.OrderBy(rm => rm.Naziv).ToListAsync();
            ViewBag.TipoviNaknada = await _context.TipoviNaknada.OrderBy(t => t.Naziv).ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UrediZaposlenika(int id) {
            var zaposlenik = await _context.Zaposlenici.FindAsync(id);
            if (zaposlenik == null) return NotFound();
            return View(zaposlenik);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiZaposlenika(int id) {
            var zaposlenik = await _context.Zaposlenici.FirstOrDefaultAsync(z => z.IdZaposlenik == id);
            if (zaposlenik == null) return NotFound();
            return View(zaposlenik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiZaposlenika(int id, IFormCollection collection) {
            try {
                var zaposlenik = await _context.Zaposlenici.FindAsync(id);

                if (zaposlenik != null) {
                    _context.Zaposlenici.Remove(zaposlenik);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Zaposlenik uspješno izbrisan sa svim povezanim podacima!";
                }
                return RedirectToAction(nameof(Zaposlenici));
            } catch (Exception ex) {
                TempData["ErrorMessage"] = $"Greška pri brisanju zaposlenika: {ex.Message}";
                return RedirectToAction(nameof(IzbrisiZaposlenika), new { id });
            }
        }

        // ZAPOSLENJE - DODAJ RADNO MJESTO ZAPOSLENIKU
        [HttpGet]
        public async Task<IActionResult> DodajRadnoMjestoZaposleniku(int? id) {
            if (id == null) return NotFound();

            var zaposlenik = await _context.Zaposlenici.FindAsync(id);
            if (zaposlenik == null) return NotFound();

            ViewBag.RadnaMjesta = await _context.RadnaMjesta.OrderBy(rm => rm.Naziv).ToListAsync();
            ViewBag.Zaposlenik = zaposlenik;

            return View(new Zaposlenje { IdZaposlenik = id.Value, DatumOd = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajRadnoMjestoZaposleniku(Zaposlenje zaposlenje) {
            if (ModelState.IsValid) {
                var postojecaAktivna = await _context.Zaposlenja
                    .Where(z => z.IdZaposlenik == zaposlenje.IdZaposlenik)
                    .Where(z => z.DatumDo == null || z.DatumDo >= DateTime.Today)
                    .FirstOrDefaultAsync();

                if (postojecaAktivna != null) {
                    postojecaAktivna.DatumDo = zaposlenje.DatumOd.AddDays(-1);
                    _context.Update(postojecaAktivna);
                }

                _context.Add(zaposlenje);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Radno mjesto uspješno dodano zaposleniku!";
                return RedirectToAction(nameof(Zaposlenici));
            }

            ViewBag.RadnaMjesta = await _context.RadnaMjesta.OrderBy(rm => rm.Naziv).ToListAsync();
            ViewBag.Zaposlenik = await _context.Zaposlenici.FindAsync(zaposlenje.IdZaposlenik);
            return View(zaposlenje);
        }

        // PRIKAZ POVIJESTI RADNIH MJESTA ZAPOSLENIKA
        [HttpGet]
        public async Task<IActionResult> PovijestRadnihMjesta(int? id) {
            if (id == null) return NotFound();

            var zaposlenik = await _context.Zaposlenici.FirstOrDefaultAsync(z => z.IdZaposlenik == id);
            if (zaposlenik == null) return NotFound();

            ViewBag.Zaposlenik = zaposlenik;

            var zaposlenja = await _context.Zaposlenja
                .Include(z => z.RadnoMjesto)
                .Where(z => z.IdZaposlenik == id)
                .OrderByDescending(z => z.DatumOd)
                .ToListAsync();

            return View(zaposlenja);
        }

        // ZATVORI RADNO MJESTO
        [HttpPost]
        public async Task<IActionResult> ZatvoriRadnoMjesto(int id) {
            var zaposlenje = await _context.Zaposlenja.FindAsync(id);
            if (zaposlenje != null) {
                zaposlenje.DatumDo = DateTime.Today;
                _context.Update(zaposlenje);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Radno mjesto uspješno zatvoreno!";
            }
            return RedirectToAction(nameof(PovijestRadnihMjesta), new { id = zaposlenje?.IdZaposlenik });
        }

        // RADNA MJESTA
        public async Task<IActionResult> RadnaMjesta() {
            var radnaMjesta = await _context.RadnaMjesta.OrderByDescending(rm => rm.IdRadnoMjesto).ToListAsync();
            return View(radnaMjesta);
        }

        [HttpGet]
        public IActionResult DodajRadnoMjesto() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajRadnoMjesto(RadnoMjesto radnoMjesto) {
            if (ModelState.IsValid) {
                _context.Add(radnoMjesto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Radno mjesto uspješno dodano!";
                return RedirectToAction(nameof(RadnaMjesta));
            }
            return View(radnoMjesto);
        }

        [HttpGet]
        public async Task<IActionResult> UrediRadnoMjesto(int? id) {
            if (id == null) return NotFound();
            var radnoMjesto = await _context.RadnaMjesta.FindAsync(id);
            if (radnoMjesto == null) return NotFound();
            return View(radnoMjesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediRadnoMjesto(int id, RadnoMjesto radnoMjesto) {
            if (id != radnoMjesto.IdRadnoMjesto) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(radnoMjesto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Radno mjesto uspješno ažurirano!";
                } catch (DbUpdateConcurrencyException) {
                    if (!RadnoMjestoExists(radnoMjesto.IdRadnoMjesto)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(RadnaMjesta));
            }
            return View(radnoMjesto);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiRadnoMjesto(int? id) {
            if (id == null) return NotFound();
            var radnoMjesto = await _context.RadnaMjesta.FirstOrDefaultAsync(m => m.IdRadnoMjesto == id);
            if (radnoMjesto == null) return NotFound();
            return View(radnoMjesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiRadnoMjesto(int id, IFormCollection collection) {
            var radnoMjesto = await _context.RadnaMjesta.FindAsync(id);
            if (radnoMjesto != null) {
                var imaZaposlenika = await _context.Zaposlenja.AnyAsync(z => z.IdRadnoMjesto == id);
                if (imaZaposlenika) {
                    TempData["ErrorMessage"] = "Ne možete izbrisati radno mjesto koje je povezano s zaposlenicima!";
                    return RedirectToAction(nameof(IzbrisiRadnoMjesto), new { id });
                }

                _context.RadnaMjesta.Remove(radnoMjesto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Radno mjesto uspješno izbrisano!";
            }
            return RedirectToAction(nameof(RadnaMjesta));
        }

        // NAKNADE
        public async Task<IActionResult> Naknade() {
            var naknade = await _context.TipoviNaknada.OrderByDescending(n => n.IdTipNaknade).ToListAsync();
            return View(naknade);
        }

        [HttpGet]
        public IActionResult DodajNaknadu() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajNaknadu(TipNaknade naknada) {
            if (ModelState.IsValid) {
                _context.Add(naknada);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Naknada uspješno dodana!";
                return RedirectToAction(nameof(Naknade));
            }
            return View(naknada);
        }

        [HttpGet]
        public async Task<IActionResult> UrediNaknadu(int? id) {
            if (id == null) return NotFound();
            var naknada = await _context.TipoviNaknada.FindAsync(id);
            if (naknada == null) return NotFound();
            return View(naknada);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediNaknadu(int id, TipNaknade naknada) {
            if (id != naknada.IdTipNaknade) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(naknada);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Naknada uspješno ažurirana!";
                } catch (DbUpdateConcurrencyException) {
                    if (!NaknadaExists(naknada.IdTipNaknade)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Naknade));
            }
            return View(naknada);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiNaknadu(int? id) {
            if (id == null) return NotFound();
            var naknada = await _context.TipoviNaknada.FirstOrDefaultAsync(m => m.IdTipNaknade == id);
            if (naknada == null) return NotFound();
            return View(naknada);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiNaknadu(int id, IFormCollection collection) {
            var naknada = await _context.TipoviNaknada.FindAsync(id);
            if (naknada != null) {
                _context.TipoviNaknada.Remove(naknada);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Naknada uspješno izbrisana!";
            }
            return RedirectToAction(nameof(Naknade));
        }

        // PRISUSTVA - SA AUTOMATSKIM DODJELJIVANJEM SMJENE
        public async Task<IActionResult> Prisustva() {
            var prisustva = await _context.Prisustva
                .Include(p => p.Zaposlenik)
                .Include(p => p.Smjena)
                .OrderByDescending(p => p.Datum)
                .ThenByDescending(p => p.IdPrisustvo)
                .ToListAsync();
            return View(prisustva);
        }

        [HttpGet]
        public async Task<IActionResult> DodajPrisustvo() {
            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.Smjene = await _context.Smjene.OrderBy(s => s.VrijemePocetka).ToListAsync();

            return View(new Prisustvo {
                Datum = DateTime.Today,
                VrijemeDolaska = new TimeSpan(8, 0, 0),
                VrijemeOdlaska = new TimeSpan(16, 0, 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajPrisustvo(Prisustvo prisustvo) {
            if (ModelState.IsValid) {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try {
                    // AUTOMATSKO DODJELJIVANJE SMJENE
                    var sati = prisustvo.VrijemeDolaska.Hours;
                    var minute = prisustvo.VrijemeDolaska.Minutes;
                    var ukupnoMinuta = sati * 60 + minute;

                    var smjene = await _context.Smjene.ToListAsync();

                    // Ako nema smjena, kreiraj default smjene
                    if (!smjene.Any()) {
                        await KreirajDefaultSmjene();
                        smjene = await _context.Smjene.ToListAsync();
                    }

                    Smjena odabranaSmjena = null;

                    if (ukupnoMinuta >= 1320 || ukupnoMinuta < 360) // 22:00-06:00 → Noćna
                    {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("noć") ||
                            s.NazivSmjene.ToLower().Contains("noc"));
                    } else if (ukupnoMinuta >= 840) // 14:00-22:00 → Druga
                      {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("druga"));
                    } else // 06:00-14:00 → Prva
                      {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("prva"));
                    }

                    if (odabranaSmjena == null) {
                        if (ukupnoMinuta >= 1320 || ukupnoMinuta < 360) {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 22 || s.VrijemePocetka.Hours == 0);
                        } else if (ukupnoMinuta >= 840) {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 14);
                        } else {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 6);
                        }
                    }

                    if (odabranaSmjena != null) {
                        prisustvo.IdSmjena = odabranaSmjena.IdSmjena;
                    } else {
                        var prvaSmjena = smjene.OrderBy(s => s.IdSmjena).FirstOrDefault();
                        if (prvaSmjena != null) {
                            prisustvo.IdSmjena = prvaSmjena.IdSmjena;
                        }
                    }

                    // Izračunaj ukupno odradeno
                    if (prisustvo.VrijemeOdlaska > prisustvo.VrijemeDolaska) {
                        var razlika = prisustvo.VrijemeOdlaska - prisustvo.VrijemeDolaska;
                        prisustvo.UkupnoOdradeno = (decimal)razlika.TotalHours;
                    } else {
                        var razlika = (prisustvo.VrijemeOdlaska + TimeSpan.FromHours(24)) - prisustvo.VrijemeDolaska;
                        prisustvo.UkupnoOdradeno = (decimal)razlika.TotalHours;
                    }

                    _context.Add(prisustvo);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var smjenaNaziv = odabranaSmjena?.NazivSmjene ?? "Nepoznata";
                    TempData["SuccessMessage"] = $"Prisustvo uspješno dodano! Automatski dodijeljena smjena: {smjenaNaziv}";
                    return RedirectToAction(nameof(Prisustva));
                } catch (Exception ex) {
                    await transaction.RollbackAsync();

                    string errorMessage = $"Greška pri spremanju: {ex.Message}";
                    if (ex.InnerException != null) {
                        errorMessage += $" | Inner: {ex.InnerException.Message}";
                    }

                    ModelState.AddModelError("", errorMessage);
                }
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.Smjene = await _context.Smjene.OrderBy(s => s.VrijemePocetka).ToListAsync();
            return View(prisustvo);
        }

        // METODA ZA KREIRANJE DEFAULT SMJENA
        private async Task KreirajDefaultSmjene() {
            try {
                var smjene = new List<Smjena>
                {
                    new Smjena
                    {
                        NazivSmjene = "Prva smjena",
                        VrijemePocetka = new TimeSpan(6, 0, 0),
                        VrijemeZavrsetka = new TimeSpan(14, 0, 0),
                        KoeficijentPlacanja = 1.0m
                    },
                    new Smjena
                    {
                        NazivSmjene = "Druga smjena",
                        VrijemePocetka = new TimeSpan(14, 0, 0),
                        VrijemeZavrsetka = new TimeSpan(22, 0, 0),
                        KoeficijentPlacanja = 1.0m
                    },
                    new Smjena
                    {
                        NazivSmjene = "Noćna smjena",
                        VrijemePocetka = new TimeSpan(22, 0, 0),
                        VrijemeZavrsetka = new TimeSpan(6, 0, 0),
                        KoeficijentPlacanja = 1.5m
                    }
                };

                _context.Smjene.AddRange(smjene);
                await _context.SaveChangesAsync();
            } catch (Exception) {
                // Bez ispisa greške
            }
        }

        [HttpGet]
        public async Task<IActionResult> UrediPrisustvo(int? id) {
            if (id == null) return NotFound();

            var prisustvo = await _context.Prisustva
                .Include(p => p.Zaposlenik)
                .Include(p => p.Smjena)
                .FirstOrDefaultAsync(p => p.IdPrisustvo == id);

            if (prisustvo == null) return NotFound();

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.Smjene = await _context.Smjene.OrderBy(s => s.VrijemePocetka).ToListAsync();
            return View(prisustvo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediPrisustvo(int id, Prisustvo prisustvo) {
            if (id != prisustvo.IdPrisustvo) return NotFound();

            if (ModelState.IsValid) {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try {
                    var sati = prisustvo.VrijemeDolaska.Hours;
                    var minute = prisustvo.VrijemeDolaska.Minutes;
                    var ukupnoMinuta = sati * 60 + minute;

                    var smjene = await _context.Smjene.ToListAsync();

                    Smjena odabranaSmjena = null;

                    if (ukupnoMinuta >= 1320 || ukupnoMinuta < 360) {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("noć") ||
                            s.NazivSmjene.ToLower().Contains("noc"));
                    } else if (ukupnoMinuta >= 840) {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("druga"));
                    } else {
                        odabranaSmjena = smjene.FirstOrDefault(s =>
                            s.NazivSmjene.ToLower().Contains("prva"));
                    }

                    if (odabranaSmjena == null) {
                        if (ukupnoMinuta >= 1320 || ukupnoMinuta < 360) {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 22 || s.VrijemePocetka.Hours == 0);
                        } else if (ukupnoMinuta >= 840) {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 14);
                        } else {
                            odabranaSmjena = smjene.FirstOrDefault(s =>
                                s.VrijemePocetka.Hours == 6);
                        }
                    }

                    if (odabranaSmjena != null) {
                        prisustvo.IdSmjena = odabranaSmjena.IdSmjena;
                    }

                    if (prisustvo.BrojSati.HasValue && prisustvo.BrojSati.Value > 0) {
                        prisustvo.UkupnoOdradeno = prisustvo.BrojSati.Value;
                    } else if (prisustvo.VrijemeDolaska != TimeSpan.Zero && prisustvo.VrijemeOdlaska != TimeSpan.Zero) {
                        if (prisustvo.VrijemeOdlaska > prisustvo.VrijemeDolaska) {
                            var razlika = prisustvo.VrijemeOdlaska - prisustvo.VrijemeDolaska;
                            prisustvo.UkupnoOdradeno = (decimal)razlika.TotalHours;
                        } else {
                            var razlika = (prisustvo.VrijemeOdlaska + TimeSpan.FromHours(24)) - prisustvo.VrijemeDolaska;
                            prisustvo.UkupnoOdradeno = (decimal)razlika.TotalHours;
                        }
                    }

                    _context.Update(prisustvo);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = $"Prisustvo uspješno ažurirano! Automatski dodijeljena smjena: {odabranaSmjena?.NazivSmjene ?? "Nepoznata"}";
                    return RedirectToAction(nameof(Prisustva));
                } catch (DbUpdateConcurrencyException) {
                    await transaction.RollbackAsync();
                    if (!PrisustvoExists(prisustvo.IdPrisustvo))
                        return NotFound();
                    throw;
                }
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.Smjene = await _context.Smjene.OrderBy(s => s.VrijemePocetka).ToListAsync();
            return View(prisustvo);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiPrisustvo(int? id) {
            if (id == null) return NotFound();

            var prisustvo = await _context.Prisustva
                .Include(p => p.Zaposlenik)
                .Include(p => p.Smjena)
                .FirstOrDefaultAsync(m => m.IdPrisustvo == id);

            if (prisustvo == null) return NotFound();

            return View(prisustvo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiPrisustvo(int id) {
            try {
                var prisustvo = await _context.Prisustva
                    .Include(p => p.Zaposlenik)
                    .FirstOrDefaultAsync(p => p.IdPrisustvo == id);

                if (prisustvo == null) {
                    TempData["ErrorMessage"] = "Prisustvo nije pronađeno!";
                    return RedirectToAction(nameof(Prisustva));
                }

                prisustvo.IdSmjena = null;
                _context.Update(prisustvo);
                await _context.SaveChangesAsync();

                _context.Prisustva.Remove(prisustvo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Prisustvo za {prisustvo.Zaposlenik?.Prezime} {prisustvo.Zaposlenik?.Ime} uspješno izbrisano!";
                return RedirectToAction(nameof(Prisustva));

            } catch (Exception ex) {
                string errorMessage = $"Greška pri brisanju: {ex.Message}";
                if (ex.InnerException != null) {
                    errorMessage += $"<br>Detalji: {ex.InnerException.Message}";
                }

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(IzbrisiPrisustvo), new { id });
            }
        }

        // ODSUSTVA
        public async Task<IActionResult> Odsustva() {
            var odsustva = await _context.Odsustva
                .Include(o => o.Zaposlenik)
                .Include(o => o.TipOdsustva)
                .OrderByDescending(o => o.DatumOd)
                .ThenByDescending(o => o.IdOdsustvo)
                .ToListAsync();
            return View(odsustva);
        }

        [HttpGet]
        public async Task<IActionResult> DodajOdsustvo() {
            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.TipoviOdsustva = await _context.TipoviOdsustva.OrderBy(t => t.Naziv).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajOdsustvo(Odsustvo odsustvo) {
            if (ModelState.IsValid) {
                try {
                    _context.Add(odsustvo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Odsustvo uspješno dodano!";
                    return RedirectToAction(nameof(Odsustva));
                } catch (Exception ex) {
                    ModelState.AddModelError("", $"Greška pri spremanju: {ex.Message}");
                }
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.TipoviOdsustva = await _context.TipoviOdsustva.OrderBy(t => t.Naziv).ToListAsync();
            return View(odsustvo);
        }

        [HttpGet]
        public async Task<IActionResult> UrediOdsustvo(int? id) {
            if (id == null) return NotFound();

            var odsustvo = await _context.Odsustva
                .Include(o => o.Zaposlenik)
                .Include(o => o.TipOdsustva)
                .FirstOrDefaultAsync(o => o.IdOdsustvo == id);
            if (odsustvo == null) return NotFound();

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.TipoviOdsustva = await _context.TipoviOdsustva.OrderBy(t => t.Naziv).ToListAsync();
            return View(odsustvo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediOdsustvo(int id, Odsustvo odsustvo) {
            if (id != odsustvo.IdOdsustvo) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(odsustvo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Odsustvo uspješno ažurirano!";
                } catch (DbUpdateConcurrencyException) {
                    if (!OdsustvoExists(odsustvo.IdOdsustvo)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Odsustva));
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            ViewBag.TipoviOdsustva = await _context.TipoviOdsustva.OrderBy(t => t.Naziv).ToListAsync();
            return View(odsustvo);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiOdsustvo(int? id) {
            if (id == null) return NotFound();

            var odsustvo = await _context.Odsustva
                .Include(o => o.Zaposlenik)
                .Include(o => o.TipOdsustva)
                .FirstOrDefaultAsync(m => m.IdOdsustvo == id);
            if (odsustvo == null) return NotFound();

            return View(odsustvo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiOdsustvo(int id, IFormCollection collection) {
            var odsustvo = await _context.Odsustva.FindAsync(id);
            if (odsustvo != null) {
                _context.Odsustva.Remove(odsustvo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Odsustvo uspješno izbrisano!";
            }
            return RedirectToAction(nameof(Odsustva));
        }

        // OBRAČUN
        public async Task<IActionResult> Obračun() {
            var obracuni = await _context.Obračuni
                .Include(o => o.Zaposlenik)
                .OrderByDescending(o => o.Godina)
                .ThenByDescending(o => o.Mjesec)
                .ThenByDescending(o => o.IdObračun)
                .ToListAsync();
            return View(obracuni);
        }

        [HttpGet]
        public async Task<IActionResult> DodajObračun() {
            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajObračun(Obračun obračun) {
            if (ModelState.IsValid) {
                if (obračun.Osnovica == 0 || obračun.BrutoPlaca == 0) {
                    var automatskiObračun = await IzračunajPlaću(obračun.IdZaposlenik, obračun.Mjesec, obračun.Godina);

                    if (automatskiObračun != null) {
                        obračun.Osnovica = automatskiObračun.Osnovica;
                        obračun.UkupneNaknade = automatskiObračun.UkupneNaknade;
                        obračun.BrutoPlaca = automatskiObračun.BrutoPlaca;
                        obračun.Porez = automatskiObračun.Porez;
                        obračun.NetoPlaca = automatskiObračun.NetoPlaca;
                        obračun.Napomene = automatskiObračun.Napomene;
                    }
                }

                _context.Add(obračun);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Obračun uspješno dodan!";
                return RedirectToAction(nameof(Obračun));
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            return View(obračun);
        }

        [HttpGet]
        public async Task<IActionResult> UrediObračun(int? id) {
            if (id == null) return NotFound();

            var obračun = await _context.Obračuni.Include(o => o.Zaposlenik).FirstOrDefaultAsync(o => o.IdObračun == id);
            if (obračun == null) return NotFound();

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            return View(obračun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediObračun(int id, Obračun obračun) {
            if (id != obračun.IdObračun) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(obračun);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Obračun uspješno ažuriran!";
                } catch (DbUpdateConcurrencyException) {
                    if (!ObračunExists(obračun.IdObračun)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Obračun));
            }

            ViewBag.Zaposlenici = await _context.Zaposlenici.OrderBy(z => z.Prezime).ThenBy(z => z.Ime).ToListAsync();
            return View(obračun);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiObračun(int? id) {
            if (id == null) return NotFound();

            var obračun = await _context.Obračuni.Include(o => o.Zaposlenik).FirstOrDefaultAsync(m => m.IdObračun == id);
            if (obračun == null) return NotFound();

            return View(obračun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiObračun(int id, IFormCollection collection) {
            var obračun = await _context.Obračuni.FindAsync(id);
            if (obračun != null) {
                _context.Obračuni.Remove(obračun);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Obračun uspješno izbrisano!";
            }
            return RedirectToAction(nameof(Obračun));
        }

        // OZNAČI ISPLAĆENO
        [HttpPost]
        public async Task<IActionResult> OznaciIsplaceno(int id) {
            var obračun = await _context.Obračuni.FindAsync(id);
            if (obračun != null) {
                obračun.Isplaceno = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Obračun označen kao isplaćen!";
            }
            return RedirectToAction(nameof(Obračun));
        }

        // JAVNA METODA ZA AJAX POZIV
        [HttpGet]
        public async Task<IActionResult> AutoIzracunajPlacu(int zaposlenikId, int mjesec, int godina) {
            try {
                var obračun = await IzračunajPlaću(zaposlenikId, mjesec, godina);

                if (obračun == null) {
                    return Json(new { success = false, message = "Zaposlenik nije pronađen." });
                }

                return Json(new {
                    success = true,
                    osnovica = obračun.Osnovica,
                    ukupneNaknade = obračun.UkupneNaknade,
                    brutoPlaca = obračun.BrutoPlaca,
                    porez = obračun.Porez,
                    netoPlaca = obračun.NetoPlaca,
                    ukupnoSati = obračun.UkupnoSati,
                    iznosOdsustva = obračun.IznosOdsustva,
                    napomene = obračun.Napomene
                });
            } catch (Exception ex) {
                return Json(new { success = false, message = $"Greška: {ex.Message}" });
            }
        }

        // GLAVNA METODA ZA IZRAČUN PLAĆE
        private async Task<Obračun?> IzračunajPlaću(int idZaposlenik, int mjesec, int godina) {
            try {
                var prviDanMjeseca = new DateTime(godina, mjesec, 1);
                var zadnjiDanMjeseca = prviDanMjeseca.AddMonths(1).AddDays(-1);

                var zaposlenik = await _context.Zaposlenici.FindAsync(idZaposlenik);
                if (zaposlenik == null) return null;

                var aktivnoZaposlenje = await _context.Zaposlenja
                    .Include(z => z.RadnoMjesto)
                    .Where(z => z.IdZaposlenik == idZaposlenik)
                    .Where(z => z.DatumOd <= zadnjiDanMjeseca)
                    .Where(z => z.DatumDo == null || z.DatumDo >= prviDanMjeseca)
                    .OrderByDescending(z => z.DatumOd)
                    .FirstOrDefaultAsync();

                if (aktivnoZaposlenje?.RadnoMjesto == null) {
                    return new Obračun {
                        IdZaposlenik = idZaposlenik,
                        Mjesec = mjesec,
                        Godina = godina,
                        Osnovica = 0,
                        UkupneNaknade = 0,
                        BrutoPlaca = 0,
                        Porez = 0,
                        NetoPlaca = 0,
                        DatumObračuna = DateTime.Now,
                        Isplaceno = false,
                        Napomene = $"Zaposlenik nema aktivno radno mjesto u mjesecu {mjesec}/{godina}."
                    };
                }

                var radnoMjesto = aktivnoZaposlenje.RadnoMjesto;
                var satnica = radnoMjesto.Satnica;

                var prisustvaUMjesecu = await _context.Prisustva
                    .Include(p => p.Smjena)
                    .Where(p => p.IdZaposlenik == idZaposlenik &&
                               p.Datum.Year == godina &&
                               p.Datum.Month == mjesec)
                    .ToListAsync();

                var naknadeZaposlenika = await _context.Naknade
                    .Include(n => n.TipNaknade)
                    .Where(n => n.IdZaposlenik == idZaposlenik)
                    .ToListAsync();

                var svaOdsustvaZaposlenika = await _context.Odsustva
                    .Include(o => o.TipOdsustva)
                    .Where(o => o.IdZaposlenik == idZaposlenik && o.Odobreno)
                    .ToListAsync();

                var ukupnoSatiPrvaIDruga = 0m;
                var ukupnoSatiNocna = 0m;
                var ukupnaOsnovica = 0m;

                foreach (var prisustvo in prisustvaUMjesecu) {
                    var koeficijent = 1.0m;
                    var odradjeniSati = prisustvo.UkupnoOdradeno;

                    if (prisustvo.Smjena != null) {
                        if (prisustvo.Smjena.NazivSmjene.ToLower().Contains("noć") ||
                            prisustvo.Smjena.NazivSmjene.ToLower().Contains("noc")) {
                            koeficijent = 1.5m;
                            ukupnoSatiNocna += odradjeniSati;
                        } else if (prisustvo.Smjena.NazivSmjene.ToLower().Contains("druga")) {
                            koeficijent = 1.0m;
                            ukupnoSatiPrvaIDruga += odradjeniSati;
                        } else {
                            koeficijent = 1.0m;
                            ukupnoSatiPrvaIDruga += odradjeniSati;
                        }
                    } else {
                        koeficijent = 1.0m;
                        ukupnoSatiPrvaIDruga += odradjeniSati;
                    }

                    var dnevnica = odradjeniSati * satnica * koeficijent;
                    ukupnaOsnovica += dnevnica;
                }

                var ukupneNaknade = 0m;
                if (naknadeZaposlenika.Any()) {
                    var brojDanaPrisustva = prisustvaUMjesecu.Select(p => p.Datum.Date).Distinct().Count();
                    var ukupneNaknadePoDanu = naknadeZaposlenika.Sum(n => n.TipNaknade?.Iznos ?? 0);
                    ukupneNaknade = ukupneNaknadePoDanu * brojDanaPrisustva;
                }

                var ukupnoDanaOdsustva = 0;
                var iznosOdsustva = 0m;

                var filtriranaOdsustvaUMjesecu = new List<Odsustvo>();
                foreach (var odsustvo in svaOdsustvaZaposlenika) {
                    if (!(odsustvo.DatumDo < prviDanMjeseca || odsustvo.DatumOd > zadnjiDanMjeseca)) {
                        filtriranaOdsustvaUMjesecu.Add(odsustvo);
                    }
                }

                foreach (var odsustvo in filtriranaOdsustvaUMjesecu.OrderBy(o => o.DatumOd)) {
                    var pocetak = odsustvo.DatumOd < prviDanMjeseca ? prviDanMjeseca : odsustvo.DatumOd;
                    var kraj = odsustvo.DatumDo > zadnjiDanMjeseca ? zadnjiDanMjeseca : odsustvo.DatumDo;

                    if (pocetak > kraj) continue;

                    int danaOdsustva = (int)(kraj - pocetak).TotalDays + 1;

                    ukupnoDanaOdsustva += danaOdsustva;
                    var koeficijentOdsustva = odsustvo.TipOdsustva?.KoeficijentIsplate ?? 1.0m;
                    var dnevnoOdsustvo = 8 * satnica * koeficijentOdsustva;
                    var iznosOvoOdsustvo = danaOdsustva * dnevnoOdsustvo;
                    iznosOdsustva += iznosOvoOdsustvo;
                }

                var brutoPlaca = ukupnaOsnovica + ukupneNaknade + iznosOdsustva;
                if (brutoPlaca < 0) brutoPlaca = 0;

                var porez = brutoPlaca * 0.25m;
                var netoPlaca = brutoPlaca - porez;

                var obračun = new Obračun {
                    IdZaposlenik = idZaposlenik,
                    Mjesec = mjesec,
                    Godina = godina,
                    Osnovica = Math.Round(ukupnaOsnovica, 2),
                    UkupneNaknade = Math.Round(ukupneNaknade, 2),
                    BrutoPlaca = Math.Round(brutoPlaca, 2),
                    Porez = Math.Round(porez, 2),
                    NetoPlaca = Math.Round(netoPlaca, 2),
                    DatumObračuna = DateTime.Now,
                    Isplaceno = false,
                    Napomene = $"Izračun za {mjesec}/{godina}. " +
                              $"{prisustvaUMjesecu.Count} prisustva, {(ukupnoSatiPrvaIDruga + ukupnoSatiNocna):F1} sati. " +
                              $"{ukupnoDanaOdsustva} dana odsustva. " +
                              $"{naknadeZaposlenika.Count} naknada."
                };

                obračun.UkupnoSati = Math.Round(ukupnoSatiPrvaIDruga + ukupnoSatiNocna, 2);
                obračun.IznosOdsustva = Math.Round(iznosOdsustva, 2);

                return obračun;
            } catch (Exception ex) {
                return new Obračun {
                    IdZaposlenik = idZaposlenik,
                    Mjesec = mjesec,
                    Godina = godina,
                    Osnovica = 0,
                    UkupneNaknade = 0,
                    BrutoPlaca = 0,
                    Porez = 0,
                    NetoPlaca = 0,
                    DatumObračuna = DateTime.Now,
                    Isplaceno = false,
                    Napomene = $"Greška pri izračunu: {ex.Message}"
                };
            }
        }

        // SMJENE
        public async Task<IActionResult> Smjene() {
            var smjene = await _context.Smjene.OrderBy(s => s.VrijemePocetka).ToListAsync();
            return View(smjene);
        }

        [HttpGet]
        public IActionResult DodajSmjenu() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajSmjenu(Smjena smjena) {
            if (ModelState.IsValid) {
                _context.Add(smjena);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Smjena uspješno dodana!";
                return RedirectToAction(nameof(Smjene));
            }
            return View(smjena);
        }

        [HttpGet]
        public async Task<IActionResult> UrediSmjenu(int? id) {
            if (id == null) return NotFound();
            var smjena = await _context.Smjene.FindAsync(id);
            if (smjena == null) return NotFound();
            return View(smjena);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediSmjenu(int id, Smjena smjena) {
            if (id != smjena.IdSmjena) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(smjena);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Smjena uspješno ažurirana!";
                } catch (DbUpdateConcurrencyException) {
                    if (!SmjenaExists(smjena.IdSmjena)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Smjene));
            }
            return View(smjena);
        }

        [HttpGet]
        public async Task<IActionResult> IzbrisiSmjenu(int? id) {
            if (id == null) return NotFound();
            var smjena = await _context.Smjene.FirstOrDefaultAsync(m => m.IdSmjena == id);
            if (smjena == null) return NotFound();
            return View(smjena);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzbrisiSmjenu(int id, IFormCollection collection) {
            var smjena = await _context.Smjene.FindAsync(id);
            if (smjena != null) {
                _context.Smjene.Remove(smjena);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Smjena uspješno izbrisana!";
            }
            return RedirectToAction(nameof(Smjene));
        }

        // POMOĆNE METODE
        private bool ZaposlenikExists(int id) => _context.Zaposlenici.Any(e => e.IdZaposlenik == id);
        private bool RadnoMjestoExists(int id) => _context.RadnaMjesta.Any(e => e.IdRadnoMjesto == id);
        private bool NaknadaExists(int id) => _context.TipoviNaknada.Any(e => e.IdTipNaknade == id);
        private bool PrisustvoExists(int id) => _context.Prisustva.Any(e => e.IdPrisustvo == id);
        private bool OdsustvoExists(int id) => _context.Odsustva.Any(e => e.IdOdsustvo == id);
        private bool ObračunExists(int id) => _context.Obračuni.Any(e => e.IdObračun == id);
        private bool SmjenaExists(int id) => _context.Smjene.Any(e => e.IdSmjena == id);
        private bool ZaposlenjeExists(int id) => _context.Zaposlenja.Any(e => e.IdZaposlenje == id);
    }
}