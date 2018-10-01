using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Extensions;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Infrastructure
{
    public class UserMessage
    {
        private UserMessage(string title, string text)
        {
            Title = title;
            Text = text;
        }

        public string Title { get; }
        
        public string Text { get; }

        public static Task PopupAction(Func<Unit, Task<IOption<UserMessage>>> action)
        {
            return action(Unit.Value).Bind(message => 
                message
                    .Map(r => Application.Current.MainPage.DisplayAlert(r.Title, r.Text, "Ok").ToUnit())
                    .GetOrElse(Tasks.Unit())
            );
        }
        
        public static UserMessage Error(string s)
        {
            return new UserMessage("Error", s);
        }
        
        public static UserMessage Error(IEnumerable<Exception> errors)
        {
            return Error(errors.Select(e => e.Message).Aggregate((s1, s2) => $"{s1}{Environment.NewLine}{s2}"));
        }

        public static UserMessage Deleted()
        {
            return new UserMessage("Success", "Successfully deleted.");
        }

        public static UserMessage Added()
        {
            return new UserMessage("Success", "Successfully added.");
        }
    }
}