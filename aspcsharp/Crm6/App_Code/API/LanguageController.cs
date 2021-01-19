using System.Collections.Generic;
using System.Web.Http;
using Crm6.App_Code.Shared;
using Helpers;

namespace API
{
    public class LanguageController : ApiController
    {

        // languages

        [AcceptVerbs("GET")]
        public bool DeleteLanguage(int id, int userId, int subscriberId)
        {
            return new Languages().DeleteLanguage(id, userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveLanguage([FromBody]LanguageSaveRequest language )
        {
            return new Languages().SaveLanguage(language);
        }


        // language phrases
        [AcceptVerbs("GET")]
        public List<LanguagePhrase> GetLanguagePhrases()
        {
            return new Languages().GetLanguagePhrases();
        }

        [AcceptVerbs("GET")]
        public bool DeleteLanguagePhrase(int id, int userId, int subscriberId)
        {
            return new Languages().DeleteLanguagePhrase(id, userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveLanguagePhrase([FromBody]LanguagePhrase languagePhrase )
        {
            return new Languages().SaveLanguagePhrase(languagePhrase);
        }


        // language translations

        [AcceptVerbs("GET")]
        public bool DeleteLanguageTranslation(int id, int userId, int subscriberId)
        {
            return new Languages().DeleteLanguageTranslation(id, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public LanguageTranslationResponse GetLanguageTranslations(int userId, int subscriberId)
        {
            // return json data of language phrase translations for user (userprofile/dispay language)
            // this is put into localstorage on the user's browser
            // used to perform do language translations on web forms with javascript - labels/page title/placeholders/messages
            return new Languages().GetLanguageTranslationData(userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveLanguageTranslation([FromBody]LanguageTranslation languageTranslation)
        {
            return new Languages().SaveLanguageTranslation(languageTranslation);
        }

        [AcceptVerbs("POST")]
        public bool VerifyLanguageTranslation([FromBody]LanguageTranslation lt)
        {
            return new Languages().VerifyLanguageTranslation(lt);
        }

        /// <summary>
        /// called from admin to setup language translations
        /// </summary>
        /// <param name="langaugeCode"></param>
        /// <param name="status"></param>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        public List<LanguageTranslation> GetLanguageTranslationsAdmin(string langaugeCode, string status, int subscriberId)
        {
            return new Languages().GetLanguageTranslations(langaugeCode, status, subscriberId);
        }

    }

}
