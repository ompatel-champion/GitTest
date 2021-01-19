using System.Collections.Generic;
using System.Linq;
using Models;
using Crm6.App_Code.Shared;
using System.Web.SessionState;
using System;


namespace Helpers
{
    public class Languages : IRequiresSessionState
    {

        #region languages

        public bool DeleteLanguage(int id, int userId, int subscriberId)
        {
            var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
            var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
            var language = sharedWriteableContext.Languages.FirstOrDefault(t => t.LanguageId == id);
            if (language != null)
            {
                sharedWriteableContext.Languages.DeleteOnSubmit(language);
                sharedWriteableContext.SubmitChanges();
                return true;
            }
            return false;
        }


        public Language GetLanguageById(int id)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var language = sharedContext.Languages.FirstOrDefault(l => l.LanguageId == id);
            return language;
        }


        public string GetLanguageName(string languageCode)
        {
            var languageName = "";
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            languageName = sharedContext.Languages.Where(l => l.LanguageCode == languageCode)
                    .Select(l => languageName)
                    .FirstOrDefault() ?? "English";
            return languageName;
        }


        public List<Language> GetLanguages()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Languages.OrderBy(l => l.LanguageCode).ToList();
        }


        public List<SelectList> PopulateLanguagesDropdown()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Languages.Select(l => new SelectList
            {
                SelectText = l.LanguageName,
                SelectValue = l.LanguageCode
            }).OrderBy(l => l.SelectText).ToList();
        }


        public int SaveLanguage(LanguageSaveRequest lp)
        {
            var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(lp.SubscriberId);
            var sharedContext = new DbSharedDataContext(sharedWritableConnection);
            var language = sharedContext.Languages.FirstOrDefault(t => t.LanguageId == lp.Language.LanguageId) ?? new Language();
            if (language != null)
            {
                language.LanguageCode = lp.Language.LanguageCode;
                language.LanguageName = lp.Language.LanguageName;

                if (language.LanguageId < 1)
                {
                    sharedContext.Languages.InsertOnSubmit(language);
                }
                sharedContext.SubmitChanges();
                return language.LanguageId;
            }

            return 0;
        }

        #endregion


        #region language phrases

        public bool DeleteLanguagePhrase(int id, int userId, int subscriberId)
        {
            var sharedWritableConnection = LoginUser.GetSharedConnection();
            var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
            var phrase = sharedWriteableContext.LanguagePhrases.FirstOrDefault(t => t.LanguagePhraseId == id);
            if (phrase != null)
            {
                phrase.Deleted = true;
                phrase.DeletedByUserId = userId;
                phrase.DeletedByUserName = new Users().GetUserFullNameById(userId, subscriberId);
                phrase.DeletedDate = DateTime.Now;
                sharedWriteableContext.SubmitChanges();
                return true;
            }
            return false;
        }


        public LanguagePhrase GetLanguagePhrase(int id)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.LanguagePhrases.FirstOrDefault(t => t.LanguagePhraseId == id);
        }


        public List<LanguagePhrase> GetLanguagePhrases()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.LanguagePhrases.Where(t => !t.Deleted).OrderBy(l => l.LanguagePhrase1).ToList();
        }


        public int SaveLanguagePhrase(LanguagePhrase lp)
        {
            // get user subscriber id 
            var connection = LoginUser.GetConnection();
            var context = new Crm6.App_Code.DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == lp.UpdateUserId);
            if (user != null)
            {
                var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(user.SubscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
                var languagePhrase = sharedWriteableContext.LanguagePhrases.FirstOrDefault(t => t.LanguagePhraseId == lp.LanguagePhraseId) ?? new LanguagePhrase();
                if (languagePhrase != null)
                {
                    languagePhrase.LanguagePhrase1 = lp.LanguagePhrase1;
                    languagePhrase.LastUpdate = DateTime.Now;
                    languagePhrase.UpdateUserId = lp.UpdateUserId;
                    languagePhrase.UpdateUserName = user.FullName;
                    if (languagePhrase.LanguagePhraseId < 1)
                    {
                        languagePhrase.CreatedDate = DateTime.Now;
                        languagePhrase.CreatedUserId = lp.UpdateUserId;
                        languagePhrase.CreatedUserName = languagePhrase.UpdateUserName;
                        sharedWriteableContext.LanguagePhrases.InsertOnSubmit(languagePhrase);
                    }
                    sharedWriteableContext.SubmitChanges();
                    return languagePhrase.LanguagePhraseId;
                }
            }
            return 0;
        }

        #endregion


        #region language translations

        public bool DeleteLanguageTranslation(int id, int userId, int subscriberId)
        {
            var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
            var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
            var translation = sharedWriteableContext.LanguageTranslations.FirstOrDefault(t => t.LanguageTranslationId == id);
            if (translation != null)
            {
                translation.Deleted = true;
                translation.DeletedUserId = userId;
                translation.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                translation.DeletedDate = DateTime.Now;
                sharedWriteableContext.SubmitChanges();
                return true;
            }
            return false;
        }


        public LanguageTranslationResponse GetLanguageTranslationData(int userId, int subscriberId)
        {
            // set the response
            var response = new LanguageTranslationResponse
            {
                LanguageCode = "en-US",
                LanguageTranslations = new List<Crm6.App_Code.Shared.LanguageTranslation>()
            };
            // get current user language code
            var user = new Users().GetUser(userId, subscriberId);
            if (user != null)
            {
                response.LanguageCode = user.User.LanguageCode;
            }
            // if language is 'en-US' return
            if (response.LanguageCode == "en-US")
                return response;

            // get list of translated language phrases for languageCode
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            response.LanguageTranslations = sharedContext.LanguageTranslations
                .Where(lt => lt.LanguageCode == response.LanguageCode && !lt.Deleted)
                .Select(lt => lt).OrderBy(lt => lt.LanguagePhrase).ToList();
            return response;
        }


        public List<LanguageTranslation> GetLanguageTranslations(string langaugeCode, string status, int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var languageTranslations = new List<LanguageTranslation>();

            // filter language translations by status
            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {

                    case "Not Verified":
                        // google translated language phrases for the selected language
                        languageTranslations = sharedContext.LanguageTranslations.Where(t => t.LanguageCode.Equals(langaugeCode)
                                                                                      && !t.Deleted
                                                                                      && !t.Verified)
                                                                                     .OrderBy(t => t.LanguagePhrase)
                                                                                     .ToList();
                        break;

                    case "Verified":
                        // manually verified by native language speaker for the selected language
                        languageTranslations = sharedContext.LanguageTranslations.Where(t => t.LanguageCode.Equals(langaugeCode)
                                                                                      && !t.Deleted && t.Verified)
                                                                                     .OrderBy(t => t.LanguagePhrase)
                                                                                     .ToList();
                        break;

                    case "New":
                        // language phrases missing translation for the selected language
                        var phrases = (from j in sharedContext.LanguagePhrases.Where(j => !j.Deleted)
                                       join t in sharedContext.LanguageTranslations.Where(t => !t.Deleted && t.LanguageCode.Equals(langaugeCode))
                                       on j.LanguagePhrase1 equals t.LanguagePhrase into untranslatedList
                                       from u in untranslatedList.DefaultIfEmpty()
                                       where u == null
                                       select j).ToList();

                        languageTranslations = phrases.Select(t => new LanguageTranslation { LanguagePhrase = t.LanguagePhrase1 }).ToList();
                        break;
                    default:
                        // not verified - google translated language phrases for the selected language
                        languageTranslations = sharedContext.LanguageTranslations.Where(t => t.LanguageCode.Equals(langaugeCode)
                                                                                      && !t.Deleted
                                                                                      && !t.Verified)
                                                                                     .OrderBy(t => t.LanguagePhrase)
                                                                                     .ToList();
                        break;
                }
            }

            return languageTranslations;
        }


        public string GetTranslatedPhrase(string languageCode, string phraseToTranslate)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var translatedPhrase = sharedContext.LanguageTranslations.Where(l => l.LanguageCode.Equals(languageCode)
                                                                           && l.LanguagePhrase.Equals(phraseToTranslate))
                                                                    .Select(l => l.Translation)
                                                                    .FirstOrDefault() ?? "";
            return translatedPhrase;
        }


        public LanguageTranslation GetTranslatedPhraseById(int id)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var translatedPhrase = sharedContext.LanguageTranslations.Where(l => l.LanguageTranslationId == id)
                                                                    .FirstOrDefault();
            return translatedPhrase;
        }


        public int SaveLanguageTranslation(LanguageTranslation lt)
        {
            // get verifying user subscriber id 
            var connection = LoginUser.GetConnection();
            var context = new Crm6.App_Code.DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == lt.UpdateUserId);
            if (user != null)
            {
                var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(user.SubscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
                var languageTranslation = sharedWriteableContext.LanguageTranslations
                    .FirstOrDefault(t => t.LanguageTranslationId == lt.LanguageTranslationId) ?? new LanguageTranslation();
                if (languageTranslation != null && !string.IsNullOrWhiteSpace(lt.Translation))
                {
                    languageTranslation.LanguageCode = lt.LanguageCode;
                    languageTranslation.LanguageName = new Languages().GetLanguageName(lt.LanguageCode);
                    languageTranslation.LanguagePhrase = lt.LanguagePhrase;
                    languageTranslation.LastUpdate = DateTime.Now;
                    languageTranslation.Translation = lt.Translation;
                    languageTranslation.UpdateUserId = lt.UpdateUserId;
                    languageTranslation.UpdateUserName = user.FullName;
                    //  languageTranslation.Verified = lt.Verified;
                    if (languageTranslation.LanguageTranslationId < 1)
                    {
                        languageTranslation.CreatedDate = DateTime.Now;
                        languageTranslation.CreatedUserId = lt.UpdateUserId;
                        languageTranslation.CreatedUserName = languageTranslation.UpdateUserName;
                        sharedWriteableContext.LanguageTranslations.InsertOnSubmit(languageTranslation);
                    }
                    sharedWriteableContext.SubmitChanges();
                    return languageTranslation.LanguageTranslationId;
                }
            }
            return 0;
        }


        public bool VerifyLanguageTranslation(LanguageTranslation lt)
        {
            // get verifying user subscriber id 
            var connection = LoginUser.GetConnection();
            var context = new Crm6.App_Code.DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == lt.VerifiedUserId);
            if (user != null)
            {
                var sharedWritableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(user.SubscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWritableConnection);
                var languageTranslation = sharedWriteableContext.LanguageTranslations.FirstOrDefault(t => t.LanguageTranslationId == lt.LanguageTranslationId);
                if (languageTranslation != null)
                {
                    languageTranslation.Translation = lt.Translation;
                    languageTranslation.Verified = true;
                    languageTranslation.VerifiedUserId = lt.VerifiedUserId;
                    languageTranslation.VerifiedUserName = user.FullName;
                    languageTranslation.UpdateUserId = lt.VerifiedUserId;
                    languageTranslation.UpdateUserName = user.FullName;
                    sharedWriteableContext.SubmitChanges();
                    return true;
                }
            }
            return false;

        }
        #endregion

    }


    // language translation model class / struct
    public class LanguageTranslationResponse
    {
        public string LanguageCode { get; set; }
        public List<LanguageTranslation> LanguageTranslations { get; set; }
    }

    public class LanguageSaveRequest
    {
        public Language Language { get; set; }
        public int SubscriberId { get; set; }
    }


}
