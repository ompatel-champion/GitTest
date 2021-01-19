using Microsoft.VisualStudio.TestTools.UnitTesting;
using Crm6.Activities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Crm6.Activities.Models;

namespace Crm6.Activities.Helpers.Tests
{
    [TestClass()]
    public class ActivityTrendsTests
    {
        private readonly ActivityTrends activityTrends;
        private readonly IDatedActivityEntityProviderFactory datedActivityEntityProviderFactory;
        private readonly IDatedActivityProvider datedActivityProvider;

        public ActivityTrendsTests()
        {
            activityTrends = new ActivityTrends(A.Fake<IReadOnlyDictionary<ActivityTrendItemType, Func<IDatedActivityProvider>>>());

            A.CallTo(() => datedActivityEntityProviderFactory.Create(ActivityTrendItemType.Events))
                .Returns(datedActivityProvider = A.Fake<IDatedActivityProvider>());
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAsyncTestDynamicData), DynamicDataSourceType.Method)]
        public async Task GetAsyncTest(IEnumerable<DatedActivityModel> datedActivityModels, int labelCount, int months, int count)
        {
            A.CallTo(() => datedActivityProvider.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                .Returns(datedActivityModels);

            var result = await activityTrends.GetAsync(new GetActivityTrendsModel { LabelCount = labelCount, Months = months });

            Assert.AreEqual(count, result.Count());
        }

        private static IEnumerable<object[]> GetAsyncTestDynamicData()
        {
            yield return new object[] { new DatedActivityModel[] { new DatedActivityModel { Date = new DateTime(2000, 1, 1) }, new DatedActivityModel { Date = new DateTime(2000, 1, 2) } }, 3, 10, 3 };
        }

        public class GetAsyncTestModel
        {
            public int Count { get; set; }
            public DatedActivityModel[] Items { get; set; }
            public int LabelCount { get; set; }
            public int Months { get; set; }
        }
    }
}