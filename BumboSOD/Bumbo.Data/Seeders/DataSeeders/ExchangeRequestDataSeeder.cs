using Bogus;
using Bumbo.Data.Models;
using Bumbo.Data.Seeders.Interfaces;

namespace Bumbo.Data.Seeders.DataSeeders;

public abstract class ExchangeRequestDataSeeder : IDataSeeder
{
    private static int Id { get; set; }

    public static IEnumerable<ExchangeRequest> GenerateExchangeRequests(List<Shift> shifts, int amountOfBranches, string locale)
    {
        var exchangeList = new List<ExchangeRequest>();
        // Not enough shifts to generate exchange requests
        if (shifts.Count < 20) return exchangeList;

        var futureShifts = shifts.Where(s => s.Start > DateTime.Now).ToList();
        var usedShiftIds = new List<int>();

        for (var i = 0; i < 15 * amountOfBranches; i++)
        {
            // Get shift that doesn't have an exchange request yet
            Shift randomShift;
            do
            {
                randomShift = new Faker().PickRandom(futureShifts);
            } while (usedShiftIds.Contains(randomShift.Id));

            usedShiftIds.Add(randomShift.Id);

            // Get shift with different user
            Shift randomShift2;
            do
            {
                randomShift2 = new Faker().PickRandom(futureShifts);
            } while (randomShift2.EmployeeId == randomShift.EmployeeId);

            var exchangeRequestFaker = new Faker<ExchangeRequest>(locale)
                .RuleFor(o => o.Id, _ => ++Id)
                .RuleFor(o => o.OriginalUserId, _ => randomShift.EmployeeId)
                .RuleFor(o => o.NewUserId, f => f.Random.Int(0, 10) < 3 ? randomShift2.EmployeeId : null)
                .RuleFor(o => o.IsApprovedByManager, f => f.Random.Int(0, 10) > 3 ? null : f.Random.Bool())
                .RuleFor(o => o.ShiftId, _ => randomShift.Id);
            var exchangeRequest = exchangeRequestFaker.Generate();

            switch (i)
            {
                // First run
                case 0:
                    exchangeRequest.NewUserId = randomShift2.EmployeeId;
                    exchangeRequest.IsApprovedByManager = null;
                    break;
                case 1:
                    exchangeRequest.NewUserId = null;
                    break;
            }

            if (exchangeRequest.NewUserId == null) exchangeRequest.IsApprovedByManager = null;

            exchangeList.Add(exchangeRequest);
        }

        return exchangeList;
    }
}