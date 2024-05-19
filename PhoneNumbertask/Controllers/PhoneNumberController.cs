using Microsoft.AspNetCore.Mvc;
using PhoneNumbertask.Models;
using System.Text.RegularExpressions;

namespace PhoneNumbertask.Controllers
{
    public class PhoneNumberController : Controller
    {
        public IActionResult Index()
        {
            return View(new PhoneNumberModel());
        }


        // Here is the post method where i am getting input from user either from text field or from file upload.
        [HttpPost]
        public IActionResult Convert(PhoneNumberModel model)
        {
            if (model.InputText != null) { 
                model.FileContent = null;
            }

            //ProcessFile()


            if (string.IsNullOrWhiteSpace(model.InputText) && model.FileContent != null)
            {
                using (var reader = new StreamReader(model.FileContent.OpenReadStream()))
                {
                    model.InputText = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrWhiteSpace(model.InputText))
            {
                ModelState.AddModelError(nameof(model.InputText), "Please enter a phone number or upload a file.");
            }
            else
            {
                model.ConvertedText = ConvertWordsToNumbers(model.InputText).Replace(" ", "");

              
                model.ValidationResult = IdentifyPhoneNumberFormat(model.ConvertedText);

                if (string.IsNullOrEmpty(model.ValidationResult))
                {
                    ModelState.AddModelError(nameof(model.InputText), "Invalid phone number format.");
                }
                else
                {
                    // If ValidationResult is valid, remove any existing errors related to InputText
                    ModelState.Remove(nameof(model.InputText));
                }
            }

            if (!ModelState.IsValid)
            {
                // If ModelState is invalid, return the view with validation errors
                return View("Index", model);
            }

            // ModelState is valid, continue processing
            model.InputText = null;
            model.FileContent = null;
            return View("Index", model);

        }




        private string ConvertWordsToNumbers(string input)
        {
            // Dictionary to map English and Hindi words to their numerical equivalents
            Dictionary<string, string> wordToNumberMap = new Dictionary<string, string>
            {
                {"ZERO", "0"}, // ZERO
                {"ONE", "1"},
                {"TWO", "2"},
                {"THREE", "3"},
                {"FOUR", "4"},
                {"FIVE", "5"},
                {"SIX", "6"},
                {"SEVEN", "7"},
                {"EIGHT", "8"},
                {"NINE", "9"},
                {"शुन्य", "0"},
                {"एक", "1"},
                {"दो", "2"},
                {"तीन", "3"},
                {"चार", "4"},
                {"पांच", "5"},
                {"छह", "6"},
                {"सात", "7"},
                {"आठ", "8"},
                {"नौ", "9"}
            };

            // Replace words with their numerical equivalents
            foreach (KeyValuePair<string, string> kvp in wordToNumberMap)
            {
                input = Regex.Replace(input, kvp.Key, kvp.Value, RegexOptions.IgnoreCase);
            }

            return input;
        }

        

        private string IdentifyPhoneNumberFormat(string phoneNumber)
        {
            // Regex patterns to identify different phone number formats
            string tenDigitPattern = @"^\d{10}$";
            string countryCodePattern = @"^\+\d{1,3}-?\d{10}$";
            string withoutParenthesesPattern = @"^(\d{3}-)?\d{3}-\d{4}$";
            string withoutParenthesesAndCountryCodePattern = @"^\+?(\d{3}-)?\d{3}-\d{4}$";

            if (Regex.IsMatch(phoneNumber, tenDigitPattern))
            {
                return "Valid number";
            }
            else if (Regex.IsMatch(phoneNumber, countryCodePattern))
            {
                return "Valid number with country code";
            }
            else if (Regex.IsMatch(phoneNumber, withoutParenthesesAndCountryCodePattern))
            {
                return "Valid number without parentheses for area code";
            }
            else if (Regex.IsMatch(phoneNumber, withoutParenthesesPattern))
            {
                return "Valid number with parentheses for area code";
            }
            else
            {
                return "Invalid phone number format";
            }
        }


    }
}
