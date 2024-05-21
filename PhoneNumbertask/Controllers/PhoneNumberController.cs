using Microsoft.AspNetCore.Mvc;
using PhoneNumbertask.Models;
using System.Text.RegularExpressions;

namespace PhoneNumbertask.Controllers
{
    public class PhoneNumberController : Controller
    {
        string resultStringAfterCheck = "";
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

                
                var phoneNumberList = model.ConvertedText.Split(',');
                foreach (var phoneNumber in phoneNumberList)
                {
                    model.ValidationResult = IdentifyPhoneNumberFormat(phoneNumber);

                    if (string.IsNullOrEmpty(model.ValidationResult))
                    {
                        ModelState.AddModelError(nameof(model.InputText), "Invalid phone number format.");
                    }
                    else
                    {
                        // If ValidationResult is valid, remove any existing errors related to InputText
                        ModelState.Remove(nameof(model.InputText));
                    }

                    model.ValidationResult = resultStringAfterCheck;    
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
            string tenDigitPattern = @"^\d{10}$"; // Matches exactly 10 digits (e.g., 1234567890)
            string countryCodePattern = @"^\+\d{1,3}-?\d{10}$"; // Matches country code followed by 10 digits (e.g., +1-1234567890 or +11234567890)
            string dashSeparatedPattern = @"^(\d{3}-)?\d{3}-\d{4}$"; // Matches format 123-456-7890 or 456-7890
            string countryCodeAndDashSeparatedPattern = @"^\+?\d{1,3}-?(\d{3}-)?\d{3}-\d{4}$"; // Matches country code and/or area code with dashes (e.g., +1-123-456-7890)

            string result;

            if (Regex.IsMatch(phoneNumber, tenDigitPattern))
            {
                result = "Valid 10-digit number.";
            }
            else if (Regex.IsMatch(phoneNumber, countryCodePattern))
            {
                result = "Valid number with country code.";
            }
            else if (Regex.IsMatch(phoneNumber, countryCodeAndDashSeparatedPattern))
            {
                result = "Valid number with optional country code and dashes.";
            }
            else if (Regex.IsMatch(phoneNumber, dashSeparatedPattern))
            {
                result = "Valid number with dashes.";
            }
            else
            {
                result = "Invalid phone number format.";
            }

            // Concatenate the result to the result string
            //resultStringAfterCheck += $"{phoneNumber}: {result}\n";
            resultStringAfterCheck += $"{phoneNumber}: {result} ";

            return result;
        }

    }



}

