<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.RazorPages;
=======
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

namespace AutoPartesRazor.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
