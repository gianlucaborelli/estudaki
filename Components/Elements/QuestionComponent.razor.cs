using Microsoft.AspNetCore.Components;
using ProvaOnline.Models;

namespace ProvaOnline.Components.Elements
{

    public partial class QuestionComponent : ComponentBase
    {
        [Parameter]
        public QuestionDocument? Value { get; set; }

        protected bool _showAnswers = false;

    }
}
