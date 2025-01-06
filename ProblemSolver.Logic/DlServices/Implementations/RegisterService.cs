using System.Text.RegularExpressions;
using OneOf;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.DL.Auth;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class RegisterService : IRegisterService
{
    public async Task<OneOf<RegisterInfo, Failed>> RegisterAsync(SolverSettings solverSettings, HttpClient client)
    {
        string password = Guid.NewGuid().ToString()[..16];
        var random = new Random();

        string nick = $"bot_{random.Next(10000)}_{DateTime.Now.Millisecond}";
        var formFields = InitializeBaseRegisterFormData();

        formFields.Add("nick", nick);
        formFields.Add("last", $"[Bot {solverSettings.AiBot}] [{solverSettings.Language}]");
        formFields.Add("first", solverSettings.Name);
        formFields.Add("password", password);
        formFields.Add("cpassword", password);

        var content = new FormUrlEncodedContent(formFields);
        var loginResponse = await client.PostAsync("regandedit/iform.jsp?lng=en", content);

        string response = await loginResponse.Content.ReadAsStringAsync();

        //If registration is successful the response contains a hidden input with a new id for DL.
        var match = Regex.Match(response, @"<input\s+type=hidden\s+name=""id""\s+value=""(\d+)""",
            RegexOptions.IgnoreCase);

        if (match.Success)
        {
            string idValue = match.Groups[1].Value;

            return new RegisterInfo
            {
                Id = idValue,
                Name = solverSettings.Name,
                Nick = nick,
                Password = password
            };
        }

        return new Failed();
    }

    /// <summary>
    ///     It's static data that always same, but we need to send it.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, string> InitializeBaseRegisterFormData()
    {
        return new Dictionary<string, string>
        {
            { "formsent", "true" },
            { "lng", "ru" },
            { "nick_parameters", "0`16`1`false`Nick" },
            { "nick_parameters2", "67b928b708e30af4a44ab23562c6a1d5" },
            { "first_parameters", "2`0`1`true`first" },
            { "first_parameters2", "4e63b4a87f42edc3960f14c8bc3d21eb" },
            { "last_parameters", "2`0`1`true`last" },
            { "last_parameters2", "22a21c1ecad9ca08eb9f58edce7490b1" },
            { "middle", "" },
            { "middle_parameters", "0`0`1`false`middle" },
            { "middle_parameters2", "a558af718a72228a76be5c5787fbb8af" },
            { "birth_parameters", "true`Birthday" },
            { "birth_parameters2", "7c041999bf62777ef01be0b93fdfb0f1" },
            { "birthday", "7" },
            { "birthmonth", "9" },
            { "birthyear", "2005" },
            { "teacher", "" },
            { "teacher_parameters", "0`6`3`false`" },
            { "teacher_parameters2", "66140b770e633472dec19afc9c63bc98" },
            { "password_parameters", "4`16`3`true`Password" },
            { "password_parameters2", "2ae3cd7fc5b430c4ff0254bf906007d7" },
            { "cpassword_parameters", "0`0`3`true`Confirm_password" },
            { "cpassword_parameters2", "4c8d8c529c7a9446bddeaa012cfc90e3" },
            { "school", "-1" },
            { "school_parameters", "false`Institution`otherschool" },
            { "school_parameters2", "bb237503849fe92190d79cb5ddebaafa" },
            { "schoolN", "-1" },
            { "schoolN_parameters", "false`№`" },
            { "schoolN_parameters2", "663d5a8485bd998ebc19f3eceb4ab841" },
            { "otherschool", "" },
            { "otherschool_parameters", "0`0`3`false`other3" },
            { "otherschool_parameters2", "3283489602d48b5c8236a2b9b3f72070" },
            { "form", "-1" },
            { "form_parameters", "false`Form/Course`" },
            { "form_parameters2", "ab66eb209c35f3393b187523c2ce233a" },
            { "group", "-1" },
            { "group_parameters", "false`Буква/Группа`" },
            { "group_parameters2", "1e1fde7e36801825e7496ae505be3d03" },
            { "country", "17" },
            { "country_parameters", "true`country`othercountry" },
            { "country_parameters2", "ad4f5d993726d6a0a76727603c13af5a" },
            { "othercountry", "" },
            { "othercountry_parameters", "0`0`3`false`other" },
            { "othercountry_parameters2", "5a6f7a76eeabd9ecdfec6bf1222468d9" },
            { "city", "20" },
            { "city_parameters", "true`city`othercity" },
            { "city_parameters2", "9978c1b85bc9ee4fc85dd56ebdf0be4e" },
            { "othercity", "" },
            { "othercity_parameters", "0`0`3`false`other2" },
            { "othercity_parameters2", "45022a54bd8f511f95f08af9cf68b4ca" },
            { "address_parameters", "200`false`Address`false`true" },
            { "address_parameters2", "e55db6ae0a995215ebfd837fb5e09847" },
            { "address", "" },
            { "phone", "" },
            { "phone_parameters", "0`0`3`false`Phone" },
            { "phone_parameters2", "55cd7f0f883cd1776e337ef9671f3236" },
            { "email", "" },
            { "email_parameters", "0`0`3`false`E-mail" },
            { "email_parameters2", "de8d78d194c790adeb7662e11a843e5e" },
            { "add_inf_parameters", "200`false`Additional_information`false`true" },
            { "add_inf_parameters2", "c6ab4d00c406828ec56a3efb546891a2" },
            { "add_inf", "" }
        };
    }
}