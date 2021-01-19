
using System.Collections.Generic;

namespace Models
{
    public class IntercomEvents
    {
        public static readonly Dictionary<UserOnboardingStep, string> OnboardingEvents =
            new Dictionary<UserOnboardingStep, string>
            {
                {UserOnboardingStep.StartOnboarding, "start user onboarding"},
                {UserOnboardingStep.FirstLogin, "first login"},
                {UserOnboardingStep.BasicUserDetailsUpdated, "basic user details updated"},
                {UserOnboardingStep.PasswordChanged, "password changed"},
                {UserOnboardingStep.ActivatedSync, "activated sync"},
                {UserOnboardingStep.AddedFirstNewCompany, "added first new company"},
                {UserOnboardingStep.AddedFirstNewDeal, "added first new deal"},
                {UserOnboardingStep.AddedFirstNewCalendarEvent, "added first new calendar event"},
                {UserOnboardingStep.RanFirstReport, "ran first report"},
                {UserOnboardingStep.AddedSalesTeamColleague, "added a sales team colleague"},
                {UserOnboardingStep.FirstMobileSignIn, "first mobile sign-in"},
            };
    };

    public enum UserOnboardingStep
    {
        StartOnboarding = 1,    // create new user
        FirstLogin = 2,         // any login 
        BasicUserDetailsUpdated = 3,    
        PasswordChanged = 4,
        ActivatedSync= 5,
        AddedFirstNewCompany=6,
        AddedFirstNewDeal=7,
        AddedFirstNewCalendarEvent=8,
        RanFirstReport=9,
        AddedSalesTeamColleague=10,
        FirstMobileSignIn=11,
        RanDealsReport=12,
    }

    public class IntercomModel
    {
        public int SubscriberId { get; set; }
        public string SubscriberName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string Browser { get; set; }
        public string BrowserLanguage { get; set; }
        public string BrowserVersion { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CreatedDateTime { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Name { get; set; }
        public string OS { get; set; }
        public string Phone { get; set; }
        public string Region { get; set; }
        public string Tag { get; set; }
        public string Timezone { get; set; }
    }

}