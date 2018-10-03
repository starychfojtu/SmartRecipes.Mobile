using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FuncSharp;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Oop
{
    public sealed class SignInViewModel : ViewModel
    {
        private readonly UserHandler handler;

        // vsechny tridy na ceste naji konstruktor nejake jine na ktere zavisi, coz v pripade velkych class bloatuje
        // navic potrebujeme nejaky DI container
        // dale je take hodne tezke otestovat kterou koliv cast, protoze musime namockovat zavislost
        // teoreticky se da obejit jenom namockovanim http clienta a mit testovaci container ve kterem vse zaregistrujeme,
        // ale to je dost slozite delat pro kazdy test
        // dale pokud bude mit napr UserHandler zavislost na dalsi veci kvuli jine metode, tak pokud chceme jako caller
        // zavolat jenom metodu, tak musime vytvorit neco co vubec nechceme
        //
        // RESENI -> zavislosti per funkce -> pure funkce
        public SignInViewModel(UserHandler handler)
        {
            this.handler = handler;
            Email = ValidatableObject.Create<string>(
                s => true, // Validation.IsEmail
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => true, // Validation.IsPassword
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }
       
        public async Task<UserActionResult> SignIn()
        {
            // Diky netypovosti musime checkovat vsude po ceste jestli jsou to validni veci
            //
            // RESENI -> udelat to silne typove
            //
            // Vubec neresi problem vraceni arraye problemu, resilo by se klasicky nejakym necomposeble TryGetNeco
            //
            // RESENI -> Validation
            if (!Email.IsValid || !Password.IsValid)
            {
                return UserActionResult.Error(UserMessages.InvalidCredentials()); 
            }

            try
            {
                // Metoda ma absolutne skyre co dela, rika jenom, ze vzdy vrati Account, coz neni pravda
                // Zaroven rika, ze bere 2 stringy, coz taky neni pravda, compiler nas vubec neupozorni, ze by mohla spadnout
                // a muzeme tedy zapomenout na try/catch. Navic try je statement a blbe se z neho varci navratova hodnota, pokud
                // bychom ji potrebovali. metoda nam sama o sobe ani nerika, ze vola nejaky side effect, coz by bylo hezke
                //
                // RESENI -> vlozit error do typu a aby nam neumoznila vubec nic tak jako ITry
                var account = await handler.SignIn(Email.Value, Password.Value);
                await Navigation.LogIn();
                return UserActionResult.Success();
            }
            // SignIn metoda leakuj detaily, to ze to vola Api nemame jako caller vedet a pokdu se to bude menit,
            // nic nas neupozorni na to, ze se to meni, kdokoliv v kodu muze rict, ze odted se nekde vola jina exceptiona
            // a tady ji zapomene volat. Chceme tedy odchytavat nejakou jinou exceptionu , ktera je vic zamerena na handler,
            // ale zaroven ve view chceme excepiony odlisovat, pokud neni connection tak udelat neco jineho nez pri invalid requestu.
            // Musely bychom tedy udelat custom exceptionu per chyba nebo zanest tuto logiku do handleru.
            //
            // RESENI -> stejne -> vlozit error do typu
            catch (ApiException e)
            {
                return UserActionResult.Error(new UserMessage("Error", e.Message));
            }
        }

        public Task<Unit> SignUp()
        {
            return Navigation.SignUp();
        }
    }
}
