using DMS.Library.Data;
using DMS.Library.Repositories.Models;
using DMS.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace DMS.Library.Constants.Seeds
{
    /// <summary>
    /// Fills the process templates the frontend currently mocks. A type is matched by its code prefix;
    /// existing types are left untouched, only missing ones are inserted.
    /// </summary>
    public class ProcessTypeSeeder(DmsContext _context)
    {
        private record StepSeed(string Name, params string[] Documents);
        private record ProcessTypeSeed(string CodePrefix, string Name, ProcessModule Module, params StepSeed[] Steps);

        private static readonly StepSeed[] DistributionCoreSteps =
        [
            new("Zahtev za ponudu", "Zahtev za ponudu"),
            new("Dobijena ponuda", "Dobijena ponuda"),
            new("Prihvatanje ponude od Foseka", "Narudžbina po njihovoj ponudi", "Dokument o prihvatanju ponude"),
            new("Ponuda za kupca", "Ponuda za kupca"),
            new("Prihvatanje ponude kupca", "Prihvatanje ponude", "Dokument narudžbe"),
        ];

        private static readonly ProcessTypeSeed[] KnownProcessTypes =
        [
            new("TRX", "Poslovna transakcija", ProcessModule.BusinessTransaction,
                new("Zahtev za ponudu", "Zahtev kupca"),
                new("Ponuda", "Ponuda"),
                new("Verifikacija", "Potvrda ponude"),
                new("Isporuka", "Otpremnica"),
                new("Faktura", "Faktura")),

            new("DIST-D", "Distribucija Foseko: domaći kupac", ProcessModule.Distribution,
                [.. DistributionCoreSteps,
                new("Otpremnica", "Otpremnica"),
                new("Faktura", "Faktura")]),

            new("DIST-I", "Distribucija Foseko: inostrani kupac", ProcessModule.Distribution,
                [.. DistributionCoreSteps,
                new("Isporuka", "JCI", "Potvrda o kvalitetu", "Račun", "CMR", "Izjava o poreklu na računu"),
                new("Otpremnica", "Otpremnica")]),

            // Bank payments are a flat document list in the UI, so a single step
            new("BNK", "Plaćanje: domaće", ProcessModule.Bank,
                new StepSeed("Dokumentacija", "JCI", "Faktura", "Nalog 70", "Izvod iz banke")),

            new("BNK-I", "Plaćanje: inostrano", ProcessModule.Bank,
                new StepSeed("Dokumentacija", "Zahtev za kupovinu deviza", "JCI", "Faktura", "Nalog 70", "Izvod iz banke")),

            new("MNC", "Tok menice", ProcessModule.BillOfExchange,
                new("Zahtev za kupovinu deviza", "Zahtev za kupovinu deviza"),
                new("Dostava menice od strane dužnika", "Dostava menice", "Menično ovlašćenje", "Naplata menice"),
                new("Dostava menice u banku", "Dostava menice", "Nalog za isplatu"),
                new("Izvod iz banke", "Izvod iz banke")),
        ];

        public async Task SeedProcessTypesAsync()
        {
            var existing = await _context.Set<ProcessTypeEntity>()
                .Select(x => x.CodePrefix)
                .ToListAsync();

            var missing = KnownProcessTypes
                .Where(type => !existing.Contains(type.CodePrefix))
                .Select(type => new ProcessTypeEntity
                {
                    Name = type.Name,
                    Module = type.Module,
                    CodePrefix = type.CodePrefix,
                    StepDefinitions = type.Steps.Select((step, i) => new StepDefinitionEntity
                    {
                        Name = step.Name,
                        Order = i + 1,
                        DocumentRequirements = step.Documents.Select((document, j) => new DocumentRequirementEntity
                        {
                            Name = document,
                            SortOrder = j + 1,
                            IsRequired = true,
                        }).ToList(),
                    }).ToList(),
                })
                .ToList();

            if (missing.Count == 0)
                return;

            await _context.Set<ProcessTypeEntity>().AddRangeAsync(missing);
            await _context.SaveChangesAsync();
        }
    }
}
