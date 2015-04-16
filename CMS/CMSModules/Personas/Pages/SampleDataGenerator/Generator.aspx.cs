using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Personas;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

/// <summary>
/// Generates various Online marketing data. This page is for internal purposes only.
/// </summary>
public partial class CMSModules_Personas_Pages_SampleDataGenerator_Generator : CMSDeskPage
{
    #region "Infrastructure"

    private static class StaticRandom
    {
        private static Random random = new Random();


        public static int Next()
        {
            return random.Next();
        }


        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }


        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }


        public static double NextDouble()
        {
            return random.NextDouble();
        }
    }


    private static class StaticRandomDocuments
    {
        private static List<TreeNode> documents;


        public static TreeNode NextDocument()
        {
            if (documents == null)
            {
                documents = DocumentHelper.GetDocuments()
                                          .PublishedVersion()
                                          .OnSite(SiteContext.CurrentSiteID)
                                          .NestingLevel(1)
                                          .ToList();
            }
            return documents[StaticRandom.Next(documents.Count)];
        }
    }


    private static class StaticRandomUsers
    {
        private static List<UserInfo> users;


        public static UserInfo NextUser()
        {
            if (users == null)
            {
                users = UserInfoProvider.GetUsers().AsEnumerable().Where(u => u.IsInSite(SiteContext.CurrentSiteName)).ToList();
            }
            return users[StaticRandom.Next(users.Count)];
        }
    }


    private static class StaticRandomMacroRules
    {
        static readonly List<string> rules = new List<string>();

        static StaticRandomMacroRules()
        {
            //Contact age is between 50 and 100
            //and
            //Contact age is not between 1 and 14
            //and
            //  Contact is male
            //  or
            //  Contact is female
            //and
            //Current date/time is between 5/29/2010 9:40:17 AM (UTC + 01:00) and 5/29/2054 9:40:24 AM (UTC + 01:00)
            rules.Add("{%Rule(\"Contact.ContactAge.Between(ToInt(50), ToInt(100)) && !Contact.ContactAge.Between(ToInt(1), ToInt(14)) && ((Contact.ContactGender == Gender.Male) || (Contact.ContactGender == Gender.Female)) && CurrentDateTime.Between(ToSystemDateTime(\\\"5/29/2010 9:40:17 AM\\\"), ToSystemDateTime(\\\"5/29/2054 9:40:24 AM\\\"))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactAgeIsBetween\\\" ><p n=\\\"age1\\\"><t>50</t><v>50</v><r>1</r><d>enter age</d><vt>integer</vt><tv>1</tv></p><p n=\\\"age2\\\"><t>100</t><v>100</v><r>1</r><d>enter age</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactAgeIsBetween\\\" ><p n=\\\"age1\\\"><t>1</t><v>1</v><r>1</r><d>enter age</d><vt>integer</vt><tv>1</tv></p><p n=\\\"age2\\\"><t>14</t><v>14</v><r>1</r><d>enter age</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_is\\\"><t>is not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"2\\\" par=\\\"\\\" op=\\\"and\\\" /><r pos=\\\"0\\\" par=\\\"2\\\" op=\\\"and\\\" n=\\\"CMSContactIsMale\\\" ><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactIsFemale\\\" ><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"3\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSCurrentDatetimeIsInRange\\\" ><p n=\\\"date2\\\"><t>5/29/2054 9:40:24 AM</t><v>5/29/2054 9:40:24 AM</v><r>1</r><d>select date</d><vt>datetime</vt><tv>1</tv></p><p n=\\\"date1\\\"><t>5/29/2010 9:40:17 AM</t><v>5/29/2010 9:40:17 AM</v><r>1</r><d>select date</d><vt>datetime</vt><tv>1</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}");
            //Contact has done any activity in the last 5 day(s)
            //or
            //Contact has done all of the following activities in the last 50 day(s): Page visit
            //and
            //  Contact has not done any of the following activities in the last 100 day(s): Abuse report
            //  or
            //  Contact does not have product Test AAAA in wishlist
            //and
            //Contact does not have product Test pants 2 in wishlist
            rules.Add("{%Rule(\"Contact.DidActivity(null, null, ToInt(5)) || Contact.DidActivities(\\\"pagevisit\\\", ToInt(50), true) && (!Contact.DidActivities(\\\"abusereport\\\", ToInt(100), false) || !Contact.Wishlist.Exists(SKUGUID == \\\"23431752-174a-46c5-b143-ae63b1b714ef\\\")) && !Contact.Wishlist.Exists(SKUGUID == \\\"923a8b6a-40a3-40a0-be84-6d6ad736c22c\\\")\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactHasDoneAnyActivityInTheLastXDays\\\" ><p n=\\\"_perfectum\\\"><t>has</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"days\\\"><t>5</t><v>5</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p></r><r pos=\\\"1\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactHasDoneFollowingActivitiesInTheLastXDays\\\" ><p n=\\\"activities\\\"><t>Page visit</t><v>pagevisit</v><r>1</r><d>select activities</d><vt>text</vt><tv>0</tv></p><p n=\\\"days\\\"><t>50</t><v>50</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_any\\\"><t>all</t><v>true</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"_perfectum\\\"><t>has</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"2\\\" par=\\\"\\\" op=\\\"and\\\" /><r pos=\\\"0\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactHasDoneFollowingActivitiesInTheLastXDays\\\" ><p n=\\\"activities\\\"><t>Abuse report</t><v>abusereport</v><r>1</r><d>select activities</d><vt>text</vt><tv>0</tv></p><p n=\\\"days\\\"><t>100</t><v>100</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_any\\\"><t>any</t><v>false</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"_perfectum\\\"><t>has not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactHasProductInWishlist\\\" ><p n=\\\"_has\\\"><t>does not have</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"product\\\"><t>Test AAAA</t><v>23431752-174a-46c5-b143-ae63b1b714ef</v><r>1</r><d>select product</d><vt>guid</vt><tv>0</tv></p></r><r pos=\\\"3\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasProductInWishlist\\\" ><p n=\\\"_has\\\"><t>does not have</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"product\\\"><t>Test pants 2</t><v>923a8b6a-40a3-40a0-be84-6d6ad736c22c</v><r>1</r><d>select product</d><vt>guid</vt><tv>0</tv></p></r></rules>\")%}");
            //  Current month is one of the following: January, February, March, April, May, June, July, August, September, October, November, December
            //  or
            //  Current day of the week is one of the following: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
            //and
            //Contact is not male
            //or
            //  Contact is from any of the following countries Afghanistan
            //  or
            //  Contact is from any of the following countries Albania
            //  or
            //  Contact is from any of the following countries American Samoa
            //and
            //Contact has spent between 5 and 100000 in the store in the last 1000 day(s)
            rules.Add("{%Rule(\"(CurrentDateTime.Month.EqualsAny(\\\"1|2|3|4|5|6|7|8|9|10|11|12\\\".Split(\\\"|\\\")) || CurrentDateTime.DayOfWeek.EqualsAny(\\\"0|1|2|3|4|5|6\\\".Split(\\\"|\\\"))) && !(Contact.ContactGender == Gender.Male) || (Contact.IsFromCountry(\\\"Afghanistan\\\") || Contact.IsFromCountry(\\\"Albania\\\") || Contact.IsFromCountry(\\\"AmericanSamoa\\\")) && Contact.SpentMoney(ToDouble(5), ToDouble(100000), ToInt(1000))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"or\\\" /><r pos=\\\"0\\\" par=\\\"0\\\" op=\\\"or\\\" n=\\\"CMSCurrentMonthIs\\\" ><p n=\\\"months\\\"><t>January, February, March, April, May, June, July, August, September, October, November, December</t><v>1|2|3|4|5|6|7|8|9|10|11|12</v><r>1</r><d>select months</d><vt>text</vt><tv>0</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"0\\\" op=\\\"or\\\" n=\\\"CMSCurrentDayOfTheWeekIsOneOfSpecifiedDays\\\" ><p n=\\\"days\\\"><t>Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday</t><v>0|1|2|3|4|5|6</v><r>1</r><d>select days</d><vt>text</vt><tv>0</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactIsMale\\\" ><p n=\\\"_is\\\"><t>is not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"2\\\" par=\\\"\\\" op=\\\"or\\\" /><r pos=\\\"0\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactIsFromCountry\\\" ><p n=\\\"countries\\\"><t>Afghanistan</t><v>Afghanistan</v><r>1</r><d>select countries</d><vt>text</vt><tv>0</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactIsFromCountry\\\" ><p n=\\\"countries\\\"><t>Albania</t><v>Albania</v><r>1</r><d>select countries</d><vt>text</vt><tv>0</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"2\\\" par=\\\"2\\\" op=\\\"or\\\" n=\\\"CMSContactIsFromCountry\\\" ><p n=\\\"countries\\\"><t>American Samoa</t><v>AmericanSamoa</v><r>1</r><d>select countries</d><vt>text</vt><tv>0</tv></p><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"3\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSpentMoneyInTheStoreInTheLastXDays\\\" ><p n=\\\"money1\\\"><t>5</t><v>5</v><r>1</r><d>enter value</d><vt>double</vt><tv>1</tv></p><p n=\\\"days\\\"><t>1000</t><v>1000</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"money2\\\"><t>100000</t><v>100000</v><r>1</r><d>enter value</d><vt>double</vt><tv>1</tv></p><p n=\\\"_perfectum\\\"><t>has</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}");
            //Contact field E-mail address contains @
            //and
            //  Contact field First name contains Dog
            //  or
            //  Contact field First name contains Jolene
            //and
            //Contact is female
            //and
            //  Contact field E-mail address ends with .com
            //  and
            //  Contact field E-mail address does not contain .cz
            rules.Add("{%Rule(\"Contact.ContactEmail.Contains(\\\"@\\\") && (Contact.ContactFirstName.Contains(\\\"Dog\\\") || Contact.ContactFirstName.Contains(\\\"Jolene\\\")) && (Contact.ContactGender == Gender.Female) && (Contact.ContactEmail.EndsWith(\\\".com\\\") && Contact.ContactEmail.NotContains(\\\".cz\\\"))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>E-mail address</t><v>ContactEmail</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>contains</t><v>Contains</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>@</t><v>@</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r><r pos=\\\"1\\\" par=\\\"\\\" op=\\\"and\\\" /><r pos=\\\"0\\\" par=\\\"1\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>First name</t><v>ContactFirstName</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>contains</t><v>Contains</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>Dog</t><v>Dog</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r><r pos=\\\"1\\\" par=\\\"1\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>First name</t><v>ContactFirstName</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>contains</t><v>Contains</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>Jolene</t><v>Jolene</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r><r pos=\\\"2\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactIsFemale\\\" ><p n=\\\"_is\\\"><t>is</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"3\\\" par=\\\"\\\" op=\\\"and\\\" /><r pos=\\\"0\\\" par=\\\"3\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>E-mail address</t><v>ContactEmail</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>ends with</t><v>EndsWith</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>.com</t><v>.com</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r><r pos=\\\"1\\\" par=\\\"3\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>E-mail address</t><v>ContactEmail</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>does not contain</t><v>NotContains</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>.cz</t><v>.cz</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r></rules>\")%}");
            //Contact has not made at least 2 order(s)
            //and
            //Contact has not logged in in the last 7 day(s)
            //and
            //Contact has not done any activity in the last 14 day(s)
            rules.Add("{%Rule(\"!(Contact.Orders.Count >= ToInt(2)) && !Contact.LoggedIn(ToInt(7)) && !Contact.DidActivity(null, null, ToInt(14))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasMadeAtLeastXOrders\\\" ><p n=\\\"num\\\"><t>2</t><v>2</v><r>1</r><d>enter number</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_perfectum\\\"><t>has not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"1\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasLoggedInInTheLastXDays\\\" ><p n=\\\"days\\\"><t>7</t><v>7</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_perfectum\\\"><t>has not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r><r pos=\\\"2\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasDoneAnyActivityInTheLastXDays\\\" ><p n=\\\"days\\\"><t>14</t><v>14</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_perfectum\\\"><t>has not</t><v>!</v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}");
        }


        public static string Next()
        {
            return rules[StaticRandom.Next(rules.Count)];
        }
    }


    private static class StaticRandomCompanies
    {
        private static readonly List<string> companyNames = new List<string>
        {
            "xentypol",
            "grifteraagency",
            "rawerastudio",
            "webcreatives",
            "gleretexdesign",
            "zafix",
            "brevia",
            "gianto",
            "duramo",
            "fumla"
        };


        public static string NextCompanyName()
        {
            return companyNames[StaticRandom.Next(companyNames.Count)];
        }
    }




    private class FirstLetterCapitalizer
    {
        public string CapitalizeFirstLetters(string toUpper)
        {
            return string.Join(" ", toUpper.Split(' ').Where(word => word.Length > 2).Select(s => char.ToUpper(s[0]) + s.Substring(1)));
        }
    }


    private class BulkInsertion
    {
        public static void Insert(IEnumerable<BaseInfo> data)
        {
            if (data == null || !data.Any())
            {
                return;
            }

            var baseInfo = data.First();

            string tableName = DataClassInfoProvider.GetClasses().WhereEquals("ClassName", baseInfo.TypeInfo.ObjectClassName).FirstObject.ClassTableName;

            ConnectionHelper.BulkInsert(CreateDataTable(data, baseInfo), tableName);
        }


        private static IEnumerable<DataColumn> ExtractDataColumns(BaseInfo info)
        {
            var type = info.GetType();

            return from c in info.ColumnNames
                   let prop = type.GetProperty(c)
                   where prop != null
                   select new DataColumn(c, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }


        private static DataTable CreateDataTable(IEnumerable<BaseInfo> data, BaseInfo info)
        {
            DataTable table = new DataTable();

            var columns = ExtractDataColumns(info).ToArray();
            if (!columns.Any())
            {
                return table;
            }

            table.Columns.AddRange(columns);
            foreach (BaseInfo item in data)
            {
                DataRow row = table.NewRow();
                FillDataRow(item, row);
                table.Rows.Add(row);
            }

            return table;
        }


        private static void FillDataRow(BaseInfo item, DataRow row)
        {
            var ti = item.TypeInfo;

            var prefix = ti.ObjectClassName.Split('.').Last();

            foreach (string column in row.Table.Columns.OfType<DataColumn>().Select(c => c.ColumnName))
            {
                if (column == ti.GUIDColumn)
                {
                    var guid = Guid.NewGuid();
                    item.SetValue(column, guid);
                    row[column] = guid;
                }
                else if ((column == ti.TimeStampColumn) ||
                         (column == string.Format("{0}Created", prefix)))
                {
                    var date = DateTime.Now;
                    item.SetValue(column, date);
                    row[column] = date;
                }
                else
                {
                    row[column] = item.GetValue(column) ?? DBNull.Value;
                }
            }
        }
    }

    #endregion


    #region "Personal data"

    private class PersonalDataStructure
    {
        public string FirstName
        {
            get;
            set;
        }


        public string LastName
        {
            get;
            set;
        }


        public string Address
        {
            get;
            set;
        }


        public string City
        {
            get;
            set;
        }


        public UserGenderEnum Gender
        {
            get;
            set;
        }


        public string MobilePhone
        {
            get;
            set;
        }


        public string HomePhone
        {
            get;
            set;
        }


        public string ZIP
        {
            get;
            set;
        }


        public string Email
        {
            get;
            set;
        }
    }


    private interface IPersonalDataGenerator
    {
        PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null);
    }


    private class RealPersonalDataGenerator : IPersonalDataGenerator
    {
        public PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null)
        {
            string contactDataUrl = "http://api.randomuser.me/0.4/";

            if (gender != null)
            {
                contactDataUrl = URLHelper.AddParameterToUrl(contactDataUrl, "gender", gender.Value == UserGenderEnum.Female ? "female" : "male");
            }

            var serializer = new JavaScriptSerializer();
            string jsonResponse = new WebClient().DownloadString(contactDataUrl);

            dynamic response = serializer.DeserializeObject(jsonResponse);
            dynamic user = response["results"][0]["user"];

            var capitalizer = new FirstLetterCapitalizer();

            var personalData = new PersonalDataStructure
            {
                Gender = user["gender"] == "male" ? UserGenderEnum.Male : UserGenderEnum.Female,
                FirstName = capitalizer.CapitalizeFirstLetters((string)user["name"]["first"]),
                LastName = capitalizer.CapitalizeFirstLetters((string)user["name"]["last"]),
                Address = capitalizer.CapitalizeFirstLetters((string)user["location"]["street"]),
                City = capitalizer.CapitalizeFirstLetters((string)user["location"]["city"]),
                MobilePhone = user["cell"],
                HomePhone = user["phone"],
                ZIP = user["location"]["zip"],
            };

            personalData.Email = personalData.FirstName + "." + personalData.LastName + "@" + StaticRandomCompanies.NextCompanyName() + ".com";

            return personalData;
        }
    }


    private class StupidPersonalDataGenerator : IPersonalDataGenerator
    {
        public PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null)
        {
            int contactNumber = StaticRandom.Next();

            string name;
            string lastName = "Doe" + contactNumber;
            gender = gender ?? (UserGenderEnum)(contactNumber % 3);
            if (gender.Value == UserGenderEnum.Male)
            {
                name = "John";
            }
            else if (gender.Value == UserGenderEnum.Female)
            {
                name = "Jolene";
            }
            else
            {
                name = "Dog";
                gender = UserGenderEnum.Unknown;
            }


            var personalData = new PersonalDataStructure()
            {
                Gender = gender.Value,
                FirstName = name,
                LastName = lastName,
                Address = name + " street",
                City = name + " city",
                MobilePhone = contactNumber.ToString(),
                HomePhone = (contactNumber * 2).ToString(),
                ZIP = (contactNumber * 3).ToString(),
            };

            personalData.Email = personalData.FirstName + "." + personalData.LastName + "@" + StaticRandomCompanies.NextCompanyName() + ".com";

            return personalData;
        }
    }

    #endregion


    #region "Generators"

    private class SampleContactStatusesGenerator
    {
        private readonly int mSiteId;


        public SampleContactStatusesGenerator(int siteID)
        {
            mSiteId = siteID;
        }


        public void Generate()
        {
            var names = new List<string>
            {
                "VIP",
                "Interested",
                "Prospective client",
                "Not interested",
                "Waste",
            };

            foreach (var contactStatusName in names)
            {
                var cs = new ContactStatusInfo()
                {
                    ContactStatusDescription = "This client is " + contactStatusName,
                    ContactStatusDisplayName = contactStatusName,
                    ContactStatusName = ValidationHelper.GetCodeName(contactStatusName),
                    ContactStatusSiteID = mSiteId,
                };

                cs.Insert();
            }
        }
    }


    private class SampleContactsGenerator
    {
        private readonly IPersonalDataGenerator mPersonalDataGenerator;
        protected readonly int mSiteId;
        

        protected readonly List<ContactStatusInfo> mExistingContactStatuses;


        public SampleContactsGenerator(IPersonalDataGenerator personalDataGenerator, int siteID)
        {
            mPersonalDataGenerator = personalDataGenerator;
            mSiteId = siteID;
            mExistingContactStatuses = ContactStatusInfoProvider.GetContactStatuses().OnSite(mSiteId).ToList();
        }


        public void Generate(int contactsCount)
        {
            List<BaseInfo> contacts = new List<BaseInfo>();

            for (int i = 0; i < contactsCount; ++i)
            {
                contacts.Add(CreateContactInfo());
            }

            BulkInsertion.Insert(contacts);


        }


        public void GenerateMergedContacts()
        {
            var allContacts = ContactInfoProvider.GetContacts().WhereEmpty("ContactMergedWithContactID").OnSite(SiteContext.CurrentSiteID).Column("ContactID");

            allContacts.ForEachPage(contactsPage =>
            {
                var mergedContact = new List<BaseInfo>();
                var contactsList = contactsPage.ToList();

                for (int i = 0; i < contactsList.Count; i++)
                {
                    var contact = contactsList[StaticRandom.Next(0, contactsList.Count)];
                    mergedContact.Add(CreateMergedContactInfo(contact.ContactID));
                }

                BulkInsertion.Insert(mergedContact);
            }, 100000);
        }


        protected ContactInfo CreateContactInfo()
        {
            var realPerson = mPersonalDataGenerator.GeneratePersonalData();

            var contact = new ContactInfo()
            {
                ContactFirstName = realPerson.FirstName,
                ContactLastName = realPerson.LastName,
                ContactEmail = realPerson.Email,
                ContactMobilePhone = realPerson.MobilePhone,
                ContactHomePhone = realPerson.HomePhone,
                ContactAddress1 = realPerson.Address,
                ContactCity = realPerson.City,
                ContactCountryID = 271,
                ContactZIP = realPerson.ZIP,
                ContactIsAnonymous = true,
                ContactStatusID = mExistingContactStatuses[StaticRandom.Next(mExistingContactStatuses.Count)].ContactStatusID,
                ContactSiteID = mSiteId,
                ContactBirthday = DateTime.Now.AddDays(-StaticRandom.Next(720, 36500))
            };

            // Gender should be left as NULL when gender is not known. This is the same behavior when contact is normally using live site 
            if (realPerson.Gender != UserGenderEnum.Unknown)
            {
                contact.ContactGender = (int)realPerson.Gender;
            }

            return contact;
        }


        protected ContactInfo CreateMergedContactInfo(int parentId)
        {
            var contact = CreateContactInfo();
            contact.ContactMergedWithContactID = parentId;
            contact.ContactMergedWhen = DateTime.Now;

            return contact;
        }
    }


    private interface ISamplePersonasGenerator
    {
        void GeneratePersonas(int siteId);
    }


    private class StupidPersonasGenerator : ISamplePersonasGenerator
    {
        public void GeneratePersonas(int siteId)
        {
            GenerateDisabledPersona(siteId);
            GenerateStupidMalePersona(siteId);
            GenerateStupidFemalePersona(siteId);
        }

        private void GenerateDisabledPersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData();

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (disabled)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = false
            };
            persona.Insert();
        }


        private void GenerateStupidMalePersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Male);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (male)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is male",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>1</value></attribute><wherecondition>ContactGender = 1</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }


        private void GenerateStupidFemalePersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Female);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (female)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is female",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>2</value></attribute><wherecondition>ContactGender = 2</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }
    }


    private class RealPersonasGenerator : ISamplePersonasGenerator
    {
        public void GeneratePersonas(int siteId)
        {
            GenerateDisabledPersona(siteId);
            GenerateRealMalePersona(siteId);
            GenerateRealFemalePersona(siteId);
        }


        private void GenerateDisabledPersona(int siteId)
        {
            var realPerson = new RealPersonalDataGenerator().GeneratePersonalData();

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = realPerson.FirstName + " " + realPerson.LastName + " (disabled)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = false
            };
            persona.Insert();
        }


        private void GenerateRealMalePersona(int siteId)
        {
            PersonalDataStructure personalData = new RealPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Male);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = personalData.FirstName + " " + personalData.LastName + " (male)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is male",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>1</value></attribute><wherecondition>ContactGender = 1</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }


        private void GenerateRealFemalePersona(int siteId)
        {
            PersonalDataStructure personalData = new RealPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Female);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = personalData.FirstName + " " + personalData.LastName + " (female)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is female",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>2</value></attribute><wherecondition>ContactGender = 2</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }
    }


    private class SampleScoresGenerator
    {
        public void GenerateScores(int scoresCount, int siteId)
        {
            List<ScoreInfo> scores = new List<ScoreInfo>();

            for (int i = 0; i < scoresCount; i++)
            {
                ScoreInfo score = new ScoreInfo
                {
                    ScoreDisplayName = "Score #" + i,
                    ScoreName = "Score-" + Guid.NewGuid(),
                    ScoreBelongsToPersona = false,
                    ScoreEnabled = true,
                    ScoreStatus = ScoreStatusEnum.RecalculationRequired,
                    ScoreSiteID = siteId
                };
                score.Insert();
                scores.Add(score);
            }

            List<RuleInfo> rules = new List<RuleInfo>();

            foreach (var score in scores)
            {
                rules.AddRange(GenerateRules(score));
            }

            BulkInsertion.Insert(rules);
        }


        private IEnumerable<RuleInfo> GenerateRules(ScoreInfo score)
        {
            var rules = new List<RuleInfo>();

            int ruleCount = StaticRandom.Next(15, 25);
            for (int i = 0; i < ruleCount; i++)
            {
                var ruleInfo = GenerateRule(score.ScoreSiteID);
                ruleInfo.RuleScoreID = score.ScoreID;
                rules.Add(ruleInfo);
            }

            return rules;
        }


        private RuleInfo GenerateRule(int siteId)
        {
            var random = StaticRandom.NextDouble();
            RuleTypeEnum ruleType = random < 0.166 ?
                RuleTypeEnum.Attribute : random < 0.333 ?
                    RuleTypeEnum.Macro : RuleTypeEnum.Activity;

            switch (ruleType)
            {
                case RuleTypeEnum.Activity:
                    return GenerateActivityRule(siteId);

                case RuleTypeEnum.Attribute:
                    return GenerateAttributeRule(siteId);

                case RuleTypeEnum.Macro:
                    return GenerateMacroRule(siteId);
            }

            return null;
        }


        private RuleInfo GenerateActivityRule(int siteId)
        {
            return GeneratePageVisitRule(siteId);
        }


        private RuleInfo GenerateAttributeRule(int siteId)
        {
            return GenerateContactLastNameContainsRandomLetterRule(siteId);
        }


        private RuleInfo GenerateContactLastNameContainsRandomLetterRule(int siteId)
        {
            var rule = GenerateBaseRule(RuleTypeEnum.Attribute, siteId);
            char letter = (char)StaticRandom.Next(97, 122);

            rule.RuleParameter = "ContactLastName";
            rule.RuleCondition = string.Format("<condition><attribute name=\"ContactLastName\"><value>{0}</value><params><Operator>0</Operator></params></attribute><wherecondition>ISNULL([ContactLastName], '') LIKE N'%{0}%'</wherecondition></condition>", letter);
            rule.RuleDisplayName = string.Format("Attribute rule for {0} points - contact last name contains {1}", rule.RuleValue, letter);

            return rule;
        }


        private RuleInfo GenerateMacroRule(int siteId)
        {
            var rule = GenerateBaseRule(RuleTypeEnum.Macro, siteId);

            string macroCondition = MacroSecurityProcessor.AddSecurityParameters(HTMLHelper.HTMLEncode(StaticRandomMacroRules.Next()), "administrator", null);
            rule.RuleCondition = string.Format("<condition><macro><value>{0}</value></macro></condition>", macroCondition);
            rule.RuleDisplayName = string.Format("Macro rule for {0} points with complex macro rule condition", rule.RuleValue);

            return rule;
        }


        private RuleInfo GenerateBaseRule(RuleTypeEnum ruleType, int siteId)
        {
            return new RuleInfo
            {
                RuleType = ruleType,
                RuleSiteID = siteId,
                RuleValue = StaticRandom.Next(10, 1000),
                RuleIsRecurring = false,
                RuleName = "Rule-" + Guid.NewGuid()
            };
        }


        private RuleInfo GeneratePageVisitRule(int siteId)
        {
            var document = StaticRandomDocuments.NextDocument();

            string activityUrl = GetRuleConditionActivityUrlField(document);
            bool generateConditionForActivityURL = !string.IsNullOrEmpty(activityUrl);

            var rule = GenerateBaseRule(RuleTypeEnum.Activity, siteId);
            rule.RuleParameter = "pagevisit";
            rule.RuleCondition = "<condition><activity name=\"pagevisit\">" +
                                 "<field name=\"ActivityNodeID\"><value>" + document.NodeID + "</value></field>" +
                                 activityUrl +
                                 "</activity><wherecondition>(ActivityType='pagevisit') AND (ActivityNodeID=" + document.NodeID + ")" +
                                 GetWhereConditionUrlField(generateConditionForActivityURL, document) +
                                 "</wherecondition></condition>";
            rule.RuleDisplayName = string.Format("Page visit rule for {0} points", rule.RuleValue);

            return rule;
        }


        private string GetRuleConditionActivityUrlField(TreeNode document)
        {
            if (StaticRandom.NextDouble() < 0.5 && document.RelativeURL.Length > 2)
            {
                return "<field name=\"ActivityURL\"><value>" + document.RelativeURL.Substring(2) + "</value><params><operator>0</operator></params></field>";
            }

            return String.Empty;
        }


        private string GetWhereConditionUrlField(bool generateConditionForActivityURL, TreeNode document)
        {
            if (generateConditionForActivityURL)
            {
                return " AND (ISNULL([ActivityURL], '') LIKE N'%" + document.RelativeURL.Substring(2) + "%')";
            }

            return String.Empty;
        }
    }


    private class SampleActivitiesGenerator
    {
        public int GenerateActivitiesForContacts(IEnumerable<ContactInfo> contacts, int mediumActivitiesCount, List<TreeNode> treeNodes)
        {
            var activities = new List<ActivityInfo>();
            DateTime created = DateTime.Now;

            foreach (var contact in contacts)
            {
                int activitiesCount = (int)(mediumActivitiesCount * StaticRandom.NextDouble() * 2);

                for (int i = 0; i < activitiesCount; i++)
                {
                    var treeNode = treeNodes[StaticRandom.Next(treeNodes.Count)];

                    var activityInfo = new ActivityInfo
                    {
                        ActivityCreated = created,
                        ActivityType = "pagevisit",
                        ActivityActiveContactID = contact.ContactID,
                        ActivityOriginalContactID = contact.ContactID,
                        ActivitySiteID = contact.ContactSiteID,
                        ActivityTitle = "Page visit on '" + treeNode.DocumentName + "' page",
                        ActivityItemID = 0,
                        ActivityItemDetailID = 0,
                        ActivityURL = treeNode.DocumentUrlPath,
                        ActivityNodeID = treeNode.NodeID,
                        ActivityValue = "",
                        ActivityIPAddress = "123.123.456.789",
                        ActivityCampaign = "",
                        ActivityURLReferrer = treeNode.DocumentUrlPath + "-totojereferrer",
                        ActivityCulture = treeNode.DocumentCulture,
                    };

                    activities.Add(activityInfo);
                }
            }
            BulkInsertion.Insert(activities);

            var activityIds = ActivityInfoProvider.GetActivities().WhereEquals("ActivityCreated", created).Select(a => a.ActivityID);

            var pageVisits = new List<PageVisitInfo>();

            foreach (var activityId in activityIds)
            {
                var pageVisitInfo = new PageVisitInfo
                {
                    PageVisitActivityID = activityId,
                    PageVisitMVTCombinationName = "",
                    PageVisitABVariantName = "",
                    PageVisitDetail = "?totojequerystring=prase",
                };

                pageVisits.Add(pageVisitInfo);
            }

            BulkInsertion.Insert(pageVisits);

            return activities.Count;
        }
    }


    private class ContactRelationshipsGenerator
    {
        public int GenerateContactRelationships(SiteInfo site)
        {
            return GenerateContactRelationships(ContactInfoProvider.GetContacts().OnSite(site.SiteID));
        }

        private int GenerateContactRelationships(ObjectQuery<ContactInfo> contacts)
        {
            var contactsList = contacts.ToList();

            foreach (var contact in contactsList)
            {
                ContactMembershipInfoProvider.SetRelationship(MemberTypeEnum.CmsUser, StaticRandomUsers.NextUser(), contact, false);
            }

            return contactsList.Count;
        }
    }


    private class SampleContactGroupsGenerator
    {
        public int GenerateContactGroups(int contactGroupsCount)
        {
            var contactGroups = new List<ContactGroupInfo>();

            for (int i = 0; i < contactGroupsCount; i++)
            {
                var contactGroup = GenerateContactGroup(StaticRandomMacroRules.Next(), "Contact Group #" + i);
                contactGroups.Add(contactGroup);
            }

            return contactGroups.Count;
        }


        public ContactGroupInfo GenerateContactGroup(string dynamicCondition, string displayName = null, bool global = false)
        {
            var contactGroup = new ContactGroupInfo()
            {
                ContactGroupDisplayName = displayName ?? "Contact Group #" + Guid.NewGuid(),
                ContactGroupName = "testCG-" + Guid.NewGuid(),
                ContactGroupEnabled = true,
                ContactGroupSiteID = global ? 0 : SiteContext.CurrentSiteID,
                ContactGroupStatus = ContactGroupStatusEnum.ConditionChanged,
                ContactGroupDynamicCondition = MacroSecurityProcessor.AddSecurityParameters(dynamicCondition, "administrator", null),
            };
            contactGroup.Insert();

            return contactGroup;
        }
    }


    private class SampleDataGenerator
    {
        public class GenerationOptions
        {
            public bool GenerateContactStatuses
            {
                get;
                set;
            }


            public int ContactsCount
            {
                get;
                set;
            }


            public bool GenerateMergedContacts
            {
                get;
                set;
            }


            public bool ContactsWithRealNames
            {
                get;
                set;
            }


            public bool GeneratePersonas
            {
                get;
                set;
            }


            public int GenerateContactGroups
            {
                get;
                set;
            }


            public int ScoresCount
            {
                get;
                set;
            }


            public int ActivitiesForEachExistingContactCount
            {
                get;
                set;
            }


            public bool GenerateContactRelationships
            {
                get;
                set;
            }
        }


        private readonly int mSiteId;


        public Action<string> Information;
        public Action<string> Error;


        public SampleDataGenerator(int siteID)
        {
            mSiteId = siteID;
        }


        public void Generate(GenerationOptions options)
        {
            try
            {
                if (options.GenerateContactStatuses)
                {
                    if (ContactStatusInfoProvider.GetContactStatuses().OnSite(mSiteId).Any())
                    {
                        Information("Contact statuses already exists");
                    }
                    else
                    {
                        new SampleContactStatusesGenerator(mSiteId).Generate();
                        Information("Contact statuses generated");
                    }
                }
                if (options.ContactsCount > 0)
                {
                    if (options.ContactsWithRealNames && options.ContactsCount > 100)
                    {
                        Error("Contacts where not generated. \"Contacts with real names\" setting shouldn't be used for generating more than 100 contacts at once.");
                    }
                    else
                    {
                        IPersonalDataGenerator personalDataGenerator = options.ContactsWithRealNames ?
                            new RealPersonalDataGenerator() :
                            (IPersonalDataGenerator)new StupidPersonalDataGenerator();
                        SampleContactsGenerator generator = new SampleContactsGenerator(personalDataGenerator, mSiteId);
                        var batch = 10000;
                        var count = options.ContactsCount;
                        while (count > batch)
                        {
                            generator.Generate(batch);
                            count -= batch;
                        }
                        generator.Generate(count);
                        Information(options.ContactsCount + " contacts generated");
                    }
                }
                if (options.GenerateMergedContacts)
                {
                    var personalDataGenerator = new StupidPersonalDataGenerator();
                    var generator = new SampleContactsGenerator(personalDataGenerator, mSiteId);
                    generator.GenerateMergedContacts();
                    Information(options.ContactsCount + " merged contacts generated");
                }
                if (options.GenerateContactRelationships)
                {
                    var generator = new ContactRelationshipsGenerator();
                    int generated = generator.GenerateContactRelationships(SiteContext.CurrentSite);
                    Information(generated + " relationships between contact and user generated");
                }
                if (options.GeneratePersonas)
                {
                    ISamplePersonasGenerator personaGenerator = options.ContactsWithRealNames ?
                        new RealPersonasGenerator() :
                        (ISamplePersonasGenerator)new StupidPersonasGenerator();
                    personaGenerator.GeneratePersonas(mSiteId);
                    Information("Sample personas generated");
                }
                if (options.GenerateContactGroups > 0)
                {
                    int generatedCGs = new SampleContactGroupsGenerator().GenerateContactGroups(options.GenerateContactGroups);

                    Information(generatedCGs + " contact groups generated");
                }
                if (options.ScoresCount > 0)
                {
                    new SampleScoresGenerator().GenerateScores(options.ScoresCount, mSiteId);
                    Information(options.ScoresCount + " scores generated");
                }
                if (options.ActivitiesForEachExistingContactCount > 0)
                {
                    var contacts = ContactInfoProvider.GetContacts().OnSite(mSiteId);

                    var documents = DocumentHelper.GetDocuments()
                                                  .PublishedVersion()
                                                  .OnSite(mSiteId)
                                                  .ToList();

                    int activitiesGenerated = 0;
                    contacts.ForEachPage(page => activitiesGenerated += new SampleActivitiesGenerator().GenerateActivitiesForContacts(page, options.ActivitiesForEachExistingContactCount, documents), 10000);

                    Information(activitiesGenerated + " activities generated");
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }
    }

    #endregion


    #region "System.Web"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            RedirectToAccessDenied("Online Marketing sample data generator is available only to the global administrator");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ShowWarning("This is Online Marketing sample data generator. This tool shouldn't be used in production! It wasn't fully tested and using it may corrupt your data. Internet connection is needed to generate contacts with real names and personas.");

        btnGenerate.Click += GenerateBtnClickAction;
        btnRecalculateCGs.Click += RecalculateCGsBtnAction;
        btnRecalculateScoreRules.Click += btnRecalculateScoreRules_Click;
        btnGenerateAll.Click += btnGenerateAll_Click;
        btnValidate.Click += btnValidate_Click;
    }


    private void GenerateBtnClickAction(object sender, EventArgs e)
    {
        var generator = new SampleDataGenerator(SiteContext.CurrentSiteID)
        {
            Information = s => ShowInformation(s),
            Error = s => ShowError(s)
        };

        var stopwatch = Stopwatch.StartNew();

        generator.Generate(new SampleDataGenerator.GenerationOptions()
        {
            GenerateContactStatuses = chckCreateContactStatuses.Checked,
            ContactsCount = chckGenerateContacts.Checked ? txtContactsCount.Text.ToInteger(0) : 0,
            GenerateMergedContacts = chckGenerateMergedContacts.Checked,
            GenerateContactRelationships = chckGenerateRelationships.Checked,
            ContactsWithRealNames = chckContactRealNames.Checked,
            GeneratePersonas = txtGeneratePersonas.Checked,
            GenerateContactGroups = chckGenerateCGs.Checked ? txtCGsCount.Text.ToInteger(0) : 0,
            ScoresCount = chckGenerateScores.Checked ? txtScoresCount.Text.ToInteger(0) : 0,
            ActivitiesForEachExistingContactCount = chckGenerateActivities.Checked ? txtActivitiesCount.Text.ToInteger(0) : 0,
        });

        ShowInformation("Time elapsed: " + stopwatch.Elapsed);
    }


    private void RecalculateCGsBtnAction(object sender, EventArgs e)
    {
        var contactGroups = ContactGroupInfoProvider.GetContactGroups()
                                                    .WhereEquals("ContactGroupStatus", ContactGroupStatusEnum.ConditionChanged)
                                                    .OnSite(SiteContext.CurrentSiteID);

        foreach (var contactGroup in contactGroups)
        {
            // Evaluate the membership of the contact group
            ContactGroupEvaluator evaluator = new ContactGroupEvaluator();
            evaluator.ContactGroup = contactGroup;
            evaluator.Execute(null);

            // Get scheduled task and update last run time
            var task = ContactGroupRebuildTaskManager.GetScheduledTask(contactGroup);
            if (task != null)
            {
                task.TaskLastRunTime = DateTime.Now;
                TaskInfoProvider.SetTaskInfo(task);
            }
        }
        
        ShowInformation("Contact Group recalculation started");
    }


    private void btnRecalculateScoreRules_Click(object sender, EventArgs e)
    {
        var scores = ScoreInfoProvider.GetScores()
                                      .WhereEquals("ScoreStatus", ScoreStatusEnum.RecalculationRequired)
                                      .WhereEquals("ScoreBelongsToPersona", 0)
                                      .OnSite(SiteContext.CurrentSiteID);

        foreach (var score in scores)
        {
            new ScoreAsyncRecalculator(score).RunAsync();
        }

        ShowInformation("Score rules recalculation started");
    }


    public override void ShowInformation(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        var messages = new MessagesPlaceHolder();
        pnlMessages.Controls.Add(messages);

        messages.ShowInformation(text, description, tooltipText, persistent);
    }


    public override void ShowWarning(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        var messages = new MessagesPlaceHolder();
        pnlMessages.Controls.Add(messages);

        messages.ShowWarning(text, description, tooltipText);
    }

    #endregion


    #region Testing data generator


    private void btnGenerateAll_Click(object sender, EventArgs e)
    {
        if (!SystemContext.DevelopmentMode)
        {
            EnableNeededSettingsKeys();

            GenerateContactGroups(true);
        }

        GenerateContactGroups(false);

        GenerateVisitorAndContentRaterPersona();
        GenerateContacterPersona();
        GenerateDisabledPersona();

        GenerateScoringWithRules();
        GenerateScoringWithRulesAndMailNotification();
    }


    private void btnValidate_Click(object sender, EventArgs e)
    {
        ValidateCGs();
        ValidatePersonas();
        ValidateScoreRules();

        cgResultsTable.Visible = true;
        personasResultsTable.Visible = true;
        scringResultsTable.Visible = true;
    }


    private static void EnableNeededSettingsKeys()
    {
        SettingsKeyInfoProvider.SetValue("CMSEnableOnlineMarketing", true);
        SettingsKeyInfoProvider.SetValue("CMSCMGlobalContactGroups", true);
        SettingsKeyInfoProvider.SetValue("CMSMergeWithIdenticalEmail", true);
        SettingsKeyInfoProvider.SetValue("CMSCMGlobalContacts", true);
        SettingsKeyInfoProvider.SetValue("CMSMergeWithIdenticalEmail", true);
        SettingsKeyInfoProvider.SetValue("CMSMergeWithIdenticalEmailToGlobal", true);
        SettingsKeyInfoProvider.SetValue("CMSEmailQueueEnabled", true);
        SettingsKeyInfoProvider.SetValue("CMSEmailsEnabled", false);
        SettingsKeyInfoProvider.SetValue("CMSRecognizeByUserAgent", false);
        SettingsKeyInfoProvider.SetValue("CMSRecognizeByIP", false);
    }


    private static void GenerateContactGroups(bool global)
    {
        var contactGroupsGenerator = new SampleContactGroupsGenerator();

        contactGroupsGenerator.GenerateContactGroup("{%Rule(\"Contact.SubmittedForm(\\\"ContactUs\\\", ToInt(7))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSubmittedSpecifiedFormInLastXDays\\\" ><p n=\\\"_perfectum\\\"><t>has</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p><p n=\\\"days\\\"><t>7</t><v>7</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"item\\\"><t>&lt;Contact Us&gt;</t><v>ContactUs</v><r>1</r><d>select form</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}", "CG: Submited Contact us form in the last 7 days", global);
        contactGroupsGenerator.GenerateContactGroup("{%Rule(\"Contact.DidActivity(null, null, ToInt(7))\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasDoneAnyActivityInTheLastXDays\\\" ><p n=\\\"days\\\"><t>7</t><v>7</v><r>0</r><d>enter days</d><vt>integer</vt><tv>1</tv></p><p n=\\\"_perfectum\\\"><t>has</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r></rules>\")%}", "CG: Contact has done any activity in the last 7 days", global);
        contactGroupsGenerator.GenerateContactGroup("{%Rule(\"Contact.ContactEmail.Contains(\\\"@\\\")\", \"<rules><r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" ><p n=\\\"field\\\"><t>E-mail address</t><v>ContactEmail</v><r>1</r><d>select field</d><vt>text</vt><tv>0</tv></p><p n=\\\"op\\\"><t>contains</t><v>Contains</v><r>0</r><d>select operator</d><vt>text</vt><tv>0</tv></p><p n=\\\"value\\\"><t>@</t><v>@</v><r>0</r><d>enter value</d><vt>text</vt><tv>1</tv></p></r></rules>\")%}", "CG: Contact has e-mail field filled", global);
    }


    private static void GenerateScoringWithRules()
    {
        ScoreInfo score = new ScoreInfo
        {
            ScoreDisplayName = "Score " + Guid.NewGuid(),
            ScoreName = "Score-" + Guid.NewGuid(),
            ScoreBelongsToPersona = false,
            ScoreEnabled = true,
            ScoreStatus = ScoreStatusEnum.RecalculationRequired,
            ScoreSiteID = SiteContext.CurrentSiteID
        };
        score.Insert();

        GenerateBasicScoringRule(
            score,
            "Has got e-mail address filled attribute rule",
            10,
            RuleTypeEnum.Attribute,
            "<condition><attribute name=\"ContactEmail\"><value>@</value><params><Operator>0</Operator></params></attribute><wherecondition>ISNULL([ContactEmail], '') LIKE N'%@%'</wherecondition></condition>",
            "ContactEmail"
            );
        
        GenerateBasicScoringRule(
            score,
            "Has got e-mail address filled macro rule",
            10,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.ContactEmail.Contains(\\\"@\\\")\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;E-mail address&lt;/t&gt;&lt;v&gt;ContactEmail&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;contains&lt;/t&gt;&lt;v&gt;Contains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;@&lt;/t&gt;&lt;v&gt;@&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        GenerateBasicScoringRule(
            score,
            "Contact has filled \"Contact us\" form for 10p",
            10,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.SubmittedForm(\\\"ContactUs\\\", ToInt(150))\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSubmittedSpecifiedFormInLastXDays\\\" &gt;&lt;p n=\\\"_perfectum\\\"&gt;&lt;t&gt;has&lt;/t&gt;&lt;v&gt;&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operation&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"days\\\"&gt;&lt;t&gt;150&lt;/t&gt;&lt;v&gt;150&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter days&lt;/d&gt;&lt;vt&gt;integer&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"item\\\"&gt;&lt;t&gt;&amp;lt;Contact Us&amp;gt;&lt;/t&gt;&lt;v&gt;ContactUs&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select form&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        GenerateBasicScoringRule(
            score,
            "10 points per ONE page visit, NOT recurring",
            10,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );

        var pageVisitRule = GenerateBasicScoringRule(
            score,
            "10 points per every page visit, recurring, max 35 total points",
            10,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );
        pageVisitRule.RuleMaxPoints = 35;
        pageVisitRule.RuleIsRecurring = true;
        pageVisitRule.Update();
        
        var pageVisitRuleWithValidity = GenerateBasicScoringRule(
            score,
            "10 points per ONE page visit, NOT recurring, 5 days validity",
            10,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );
        pageVisitRuleWithValidity.RuleValidFor = 5;
        pageVisitRuleWithValidity.RuleValidity = ValidityEnum.Days;
        pageVisitRuleWithValidity.Update();
    }


    private static void GenerateScoringWithRulesAndMailNotification()
    {
        ScoreInfo score = new ScoreInfo
        {
            ScoreDisplayName = "Score with notification " + Guid.NewGuid(),
            ScoreName = "ScoreMailAndTrigger-" + Guid.NewGuid(),
            ScoreBelongsToPersona = false,
            ScoreEnabled = true,
            ScoreStatus = ScoreStatusEnum.RecalculationRequired,
            ScoreSiteID = SiteContext.CurrentSiteID,
            ScoreEmailAtScore = 50,
            ScoreNotificationEmail = "test@test.local"
        };
        score.Insert();

        GenerateBasicScoringRule(
            score,
            "Notification test: 10 points per ONE page visit, NOT recurring",
            10,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );

        GenerateBasicScoringRule(
            score,
            "Notification test: 15 points for Home landing page",
            15,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"landingpage\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>Home</value><params><operator>0</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='landingpage') AND (ISNULL([ActivityURL], '') LIKE N'%Home%')</wherecondition></condition>",
            "landingpage"
            );

        GenerateBasicScoringRule(
            score,
            "Notification test: Contact has filled \"Contact us\" form for 30p",
            30,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.SubmittedForm(\\\"ContactUs\\\", ToInt(150))\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSubmittedSpecifiedFormInLastXDays\\\" &gt;&lt;p n=\\\"_perfectum\\\"&gt;&lt;t&gt;has&lt;/t&gt;&lt;v&gt;&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operation&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"days\\\"&gt;&lt;t&gt;150&lt;/t&gt;&lt;v&gt;150&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter days&lt;/d&gt;&lt;vt&gt;integer&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"item\\\"&gt;&lt;t&gt;&amp;lt;Contact Us&amp;gt;&lt;/t&gt;&lt;v&gt;ContactUs&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select form&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );
    }


    private static void GenerateVisitorAndContentRaterPersona()
    {
        var persona = new PersonaInfo
        {
            PersonaDisplayName = "Page visitor and Content rater",
            PersonaName = "Persona-" + Guid.NewGuid(),
            PersonaSiteID = SiteContext.CurrentSiteID,
            PersonaPointsThreshold = 75,
            PersonaEnabled = true
        };
        persona.Insert();

        GenerateBasicRule(
            persona,
            "100 points for content rating activity",
            100,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"rating\"><field name=\"ActivityValue\"><value>0</value><params><operator>11</operator></params></field><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><params><operator>0</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='rating') AND (CASE ISNUMERIC([ActivityValue]) WHEN 1 THEN CAST([ActivityValue] AS FLOAT) ELSE NULL END &gt; CAST(0 AS FLOAT))</wherecondition></condition>",
            "rating"
            );

        GenerateBasicRule(
            persona,
            "15 points for Home landing page",
            15,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"landingpage\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>Home</value><params><operator>0</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='landingpage') AND (ISNULL([ActivityURL], '') LIKE N'%Home%')</wherecondition></condition>",
            "landingpage"
            );

        GenerateBasicRule(
            persona,
            "Always satisfied macro rule for 30 points",
            30,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.ContactLastName.Contains(\\\"Anonym\\\") || Contact.ContactLastName.NotContains(\\\"Anonym\\\")\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;contains&lt;/t&gt;&lt;v&gt;Contains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;r pos=\\\"1\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;does not contain&lt;/t&gt;&lt;v&gt;NotContains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        var pageVisitRule = GenerateBasicRule(
            persona,
            "10 points per every page visit",
            10,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );
        pageVisitRule.RuleMaxPoints = 35;
        pageVisitRule.RuleIsRecurring = true;
        pageVisitRule.Update();
    }


    private static void GenerateContacterPersona()
    {
        var persona = new PersonaInfo
        {
            PersonaDisplayName = "Contacter",
            PersonaName = "Persona-" + Guid.NewGuid(),
            PersonaSiteID = SiteContext.CurrentSiteID,
            PersonaPointsThreshold = 75,
            PersonaEnabled = true
        };
        persona.Insert();

        GenerateBasicRule(
            persona,
            "Always satisfied macro rule for 30 points",
            30,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.ContactLastName.Contains(\\\"Anonym\\\") || Contact.ContactLastName.NotContains(\\\"Anonym\\\")\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;contains&lt;/t&gt;&lt;v&gt;Contains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;r pos=\\\"1\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;does not contain&lt;/t&gt;&lt;v&gt;NotContains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        GenerateBasicRule(
            persona,
            "Contact has got e-mail address filled for 50p",
            50,
            RuleTypeEnum.Attribute,
            "<condition><attribute name=\"ContactEmail\"><value>@</value><params><Operator>0</Operator></params></attribute><wherecondition>ISNULL([ContactEmail], '') LIKE N'%@%'</wherecondition></condition>",
            "ContactEmail"
            );

        GenerateBasicRule(
            persona,
            "Contact has filled \"Contact us\" form for 50p",
            50,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.SubmittedForm(\\\"ContactUs\\\", ToInt(150))\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSubmittedSpecifiedFormInLastXDays\\\" &gt;&lt;p n=\\\"_perfectum\\\"&gt;&lt;t&gt;has&lt;/t&gt;&lt;v&gt;&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operation&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"days\\\"&gt;&lt;t&gt;150&lt;/t&gt;&lt;v&gt;150&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter days&lt;/d&gt;&lt;vt&gt;integer&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"item\\\"&gt;&lt;t&gt;&amp;lt;Contact Us&amp;gt;&lt;/t&gt;&lt;v&gt;ContactUs&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select form&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        var pageVisitRule = GenerateBasicRule(
            persona,
            "1 point per every page visit",
            1,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );
        pageVisitRule.RuleMaxPoints = 2;
        pageVisitRule.RuleIsRecurring = true;
        pageVisitRule.Update();
    }


    private static void GenerateDisabledPersona()
    {
        var persona = new PersonaInfo
        {
            PersonaDisplayName = "Disabled persona with various rules",
            PersonaName = "Persona-" + Guid.NewGuid(),
            PersonaSiteID = SiteContext.CurrentSiteID,
            PersonaPointsThreshold = 5,
            PersonaEnabled = false
        };
        persona.Insert();

        GenerateBasicRule(
            persona,
            "100 points for content rating activity",
            100,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"rating\"><field name=\"ActivityValue\"><value>0</value><params><operator>11</operator></params></field><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><params><operator>0</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='rating') AND (CASE ISNUMERIC([ActivityValue]) WHEN 1 THEN CAST([ActivityValue] AS FLOAT) ELSE NULL END &gt; CAST(0 AS FLOAT))</wherecondition></condition>",
            "rating"
            );

        GenerateBasicRule(
            persona,
            "Always satisfied macro rule for 30 points",
            30,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.ContactLastName.Contains(\\\"Anonym\\\") || Contact.ContactLastName.NotContains(\\\"Anonym\\\")\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;contains&lt;/t&gt;&lt;v&gt;Contains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;r pos=\\\"1\\\" par=\\\"\\\" op=\\\"or\\\" n=\\\"CMSContactFieldContainsValue\\\" &gt;&lt;p n=\\\"field\\\"&gt;&lt;t&gt;Last name&lt;/t&gt;&lt;v&gt;ContactLastName&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select field&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"op\\\"&gt;&lt;t&gt;does not contain&lt;/t&gt;&lt;v&gt;NotContains&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operator&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"value\\\"&gt;&lt;t&gt;Anonym&lt;/t&gt;&lt;v&gt;Anonym&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter value&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        GenerateBasicRule(
            persona,
            "Contact has got e-mail address filled for 50p",
            50,
            RuleTypeEnum.Attribute,
            "<condition><attribute name=\"ContactEmail\"><value>@</value><params><Operator>0</Operator></params></attribute><wherecondition>ISNULL([ContactEmail], '') LIKE N'%@%'</wherecondition></condition>",
            "ContactEmail"
            );

        GenerateBasicRule(
            persona,
            "Contact has filled \"Contact us\" form for 50p",
            50,
            RuleTypeEnum.Macro,
            "<condition><macro><value>{%Rule(\"Contact.SubmittedForm(\\\"ContactUs\\\", ToInt(150))\", \"&lt;rules&gt;&lt;r pos=\\\"0\\\" par=\\\"\\\" op=\\\"and\\\" n=\\\"CMSContactHasSubmittedSpecifiedFormInLastXDays\\\" &gt;&lt;p n=\\\"_perfectum\\\"&gt;&lt;t&gt;has&lt;/t&gt;&lt;v&gt;&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;select operation&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"days\\\"&gt;&lt;t&gt;150&lt;/t&gt;&lt;v&gt;150&lt;/v&gt;&lt;r&gt;0&lt;/r&gt;&lt;d&gt;enter days&lt;/d&gt;&lt;vt&gt;integer&lt;/vt&gt;&lt;tv&gt;1&lt;/tv&gt;&lt;/p&gt;&lt;p n=\\\"item\\\"&gt;&lt;t&gt;&amp;lt;Contact Us&amp;gt;&lt;/t&gt;&lt;v&gt;ContactUs&lt;/v&gt;&lt;r&gt;1&lt;/r&gt;&lt;d&gt;select form&lt;/d&gt;&lt;vt&gt;text&lt;/vt&gt;&lt;tv&gt;0&lt;/tv&gt;&lt;/p&gt;&lt;/r&gt;&lt;/rules&gt;\")%}</value></macro></condition>"
            );

        var pageVisitRule = GenerateBasicRule(
            persona,
            "5 points per every page visit",
            5,
            RuleTypeEnum.Activity,
            "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><value>XXXYYYZZZ</value><params><operator>1</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"PageVisitDetail\"><params><operator>0</operator></params></field><field name=\"PageVisitABVariantName\"><params><operator>0</operator></params></field><field name=\"PageVisitMVTCombinationName\"><params><operator>0</operator></params></field></activity><wherecondition>(ActivityType='pagevisit') AND (ISNULL([ActivityURL], '') NOT LIKE N'%XXXYYYZZZ%')</wherecondition></condition>",
            "pagevisit"
            );
        pageVisitRule.RuleMaxPoints = 100000;
        pageVisitRule.RuleIsRecurring = true;
        pageVisitRule.Update();

    }


    private static RuleInfo GenerateBasicRule(PersonaInfo persona, string displayName, int ruleValue, RuleTypeEnum ruleType, string ruleCondition, string ruleParameter = null)
    {
        var rule = new RuleInfo
        {
            RuleScoreID = persona.PersonaScoreID,
            RuleDisplayName = displayName,
            RuleName = "Rule-" + Guid.NewGuid(),
            RuleValue = ruleValue,
            RuleType = ruleType,
            RuleParameter = ruleParameter,
            RuleCondition = ruleCondition,
            RuleSiteID = persona.PersonaSiteID,
            RuleBelongsToPersona = true,
            RuleIsRecurring = false
        };
        rule.Insert();

        return rule;
    }


    private static RuleInfo GenerateBasicScoringRule(ScoreInfo score, string displayName, int ruleValue, RuleTypeEnum ruleType, string ruleCondition, string ruleParameter = null)
    {
        var rule = new RuleInfo
        {
            RuleScoreID = score.ScoreID,
            RuleDisplayName = displayName,
            RuleName = "Rule-" + Guid.NewGuid(),
            RuleValue = ruleValue,
            RuleType = ruleType,
            RuleParameter = ruleParameter,
            RuleCondition = ruleCondition,
            RuleSiteID = score.ScoreSiteID,
            RuleBelongsToPersona = false,
            RuleIsRecurring = false
        };
        rule.Insert();

        return rule;
    }


    private void ValidateCGs()
    {
        var groups = ContactGroupInfoProvider.GetContactGroups()
                                             .OnSite(SiteContext.CurrentSiteID, true);

        int siteContactsCount = ContactInfoProvider.GetContacts().OnSite(SiteContext.CurrentSiteID).NotMerged().Count;
        int sitePlusGlobalContactsCount = ContactInfoProvider.GetContacts().OnSite(SiteContext.CurrentSiteID, true).NotMerged().Count;

        foreach (var groupInfo in groups)
        {
            int numberOfContactsInGroup = ContactGroupMemberInfoProvider.GetNumberOfContactsInGroup(groupInfo.ContactGroupID);
            int totalContactsCount = groupInfo.ContactGroupSiteID == 0 ? sitePlusGlobalContactsCount : siteContactsCount;

            int mergedContactsCount = ContactGroupMemberInfoProvider.GetRelationships()
                                                                    .WhereEquals("ContactGroupMemberContactGroupID", groupInfo.ContactGroupID)
                                                                    .WhereEquals("ContactGroupMemberType", ContactGroupMemberTypeEnum.Contact)
                                                                    .Source(s => s.Join<ContactInfo>("ContactGroupMemberRelatedID", "ContactID"))
                                                                    .Where(new WhereCondition().WhereNotEmpty("ContactMergedWithContactID")
                                                                                               .Or(new WhereCondition().WhereNotEmpty("ContactGlobalContactID")
                                                                                                                       .WhereEmpty("ContactSiteID")))
                                                                    .Count();

            var tableRow = CreateTableRow(groupInfo.ContactGroupDisplayName,
                numberOfContactsInGroup.ToString(),
                (numberOfContactsInGroup / (double)totalContactsCount * 100).ToString(),
                groupInfo.IsGlobal.ToString(),
                mergedContactsCount > 0 ? "ERROR" : "OK"
                );

            cgResultsTable.Rows.Add(tableRow);
        }
    }


    private void ValidatePersonas()
    {
        var personas = PersonaInfoProvider.GetPersonas().OnSite(SiteContext.CurrentSiteID);
        int totalContactsCount = ContactInfoProvider.GetContacts().WhereEmpty("ContactMergedWithContactID").OnSite(SiteContext.CurrentSiteID).Count;

        foreach (var persona in personas)
        {
            int personaMembersCount = ContactInfoProvider.GetContacts().WhereEquals("ContactPersonaID", persona.PersonaID).Count;

            int mergedContactsCount = ContactInfoProvider.GetContacts()
                                                         .WhereEquals("ContactPersonaID", persona.PersonaID)
                                                         .WhereNotEmpty("ContactMergedWithContactID")
                                                         .Count();

            var tableRow = CreateTableRow(persona.PersonaDisplayName,
                personaMembersCount.ToString(),
                (personaMembersCount / (double)totalContactsCount * 100).ToString(),
                mergedContactsCount > 0 ? "ERROR" : "OK"
                );

            personasResultsTable.Rows.Add(tableRow);
        }

        int contactWithNoPersona = ContactInfoProvider.GetContacts()
                                                      .OnSite(SiteContext.CurrentSiteID)
                                                      .WhereEmpty("ContactPersonaID")
                                                      .WhereEmpty("ContactMergedWithContactID")
                                                      .Count;

        var tableRowNoPersona = CreateTableRow("CONTACTS IN NO PERSONA",
                contactWithNoPersona.ToString(),
                (contactWithNoPersona / (double)totalContactsCount * 100).ToString(),
                "N/A"
                );

        personasResultsTable.Rows.Add(tableRowNoPersona);
    }


    private void ValidateScoreRules()
    {
        var rules = RuleInfoProvider.GetRules().WhereEquals("RuleBelongsToPersona", 0).OnSite(SiteContext.CurrentSiteID);
        int totalContactsCount = ContactInfoProvider.GetContacts().WhereEmpty("ContactMergedWithContactID").OnSite(SiteContext.CurrentSiteID).Count;

        foreach (var rule in rules)
        {
            var scoreRules = ScoreContactRuleInfoProvider.GetScoreContactRules().WhereEquals("RuleID", rule.RuleID).ToList();
            
            var tableRow = CreateTableRow(rule.RuleDisplayName,
                scoreRules.Count.ToString(),
                string.Join("<br />", scoreRules.GroupBy(x => x.Value).Select(x => x.Key + ":" + x.Count())),
                (scoreRules.Count / (double)totalContactsCount * 100).ToString()
                );

            scringResultsTable.Rows.Add(tableRow);
        }

        var tableRowMails = CreateTableRow("# of emails waiting in Queue:",
            EmailInfoProvider.GetEmailCount(null).ToString(),
            "N/A",
            "N/A"
            );

        scringResultsTable.Rows.Add(tableRowMails);
    }


    private TableCell CreateTableCell(string cellText)
    {
        var cell = new TableCell();
        cell.Text = cellText;

        return cell;
    }


    private TableRow CreateTableRow(params string[] cellsText)
    {
        var tableRow = new TableRow();

        foreach (string cellText in cellsText)
        {
            tableRow.Cells.Add(CreateTableCell(cellText));
        }

        return tableRow;
    }

    #endregion
}