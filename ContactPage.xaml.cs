using Microsoft.Maui.Controls;

namespace _6osa
{
    // Ainult .cs fail - XAML faili pole vaja, kustuta ContactPage.xaml kui see on olemas
    public partial class ContactPage : ContentPage
    {
        // --- Kõik elemendid ---
        EntryCell? nimeCell;
        EntryCell? emailCell;
        EntryCell? telefonCell;
        EntryCell? kirjeldusCell;
        EntryCell? sõnumCell;
        SwitchCell? sc;
        ImageCell? ic;
        TableSection? fotoSection;
        Label? tervitusLabel;
        TableView? tabelview;

        // --- Tervituste nimekiri ---
        readonly List<string> tervitused = new List<string>
        {
            "🎂 Palju õnne sünnipäevaks! Soovin sulle kõike paremat!",
            "🎄 Häid jõule ja õnnelikku uut aastat!",
            "🌸 Tere kevad! Loodan, et sul on suurepärane päev!",
            "🏖️ Häid puhkusepäevi! Naudi puhkust täiel rinnal!",
            "🎓 Palju õnne lõpetamise puhul! Suur saavutus!",
            "💪 Jätka nii! Sa oled parim sõber!",
            "🌟 Sa oled täiesti eriline inimene! 🌟"
        };

        string valitudTervitus = "";

        public ContactPage()
        {
            Title = "Sõbrade kontaktiraamat";
            EhitaTabel();
        }

        void EhitaTabel()
        {
            // --- Sõbra andmed ---
            nimeCell = new EntryCell { Label = "Nimi", Placeholder = "Sisesta sõbra nimi" };
            emailCell = new EntryCell { Label = "Email", Placeholder = "Sisesta email", Keyboard = Keyboard.Email };
            telefonCell = new EntryCell { Label = "Telefon", Placeholder = "Sisesta tel. number", Keyboard = Keyboard.Telephone };
            kirjeldusCell = new EntryCell { Label = "Kirjeldus", Placeholder = "Kirjeldus sõbra kohta" };

            var andmedSection = new TableSection("Sõbra andmed")
            {
                nimeCell, emailCell, telefonCell, kirjeldusCell
            };

            // --- Foto toggle ---
            sc = new SwitchCell { Text = "Näita fotot" };
            sc.OnChanged += Sc_OnChanged;

            ic = new ImageCell
            {
                ImageSource = ImageSource.FromFile("dotnet_bot.png"),
                Text = "Sõbra foto",
                Detail = "Foto kirjeldus"
            };

            var switchSection = new TableSection("Foto") { sc };
            fotoSection = new TableSection("");

            // --- Sõnumi saatmine ---
            sõnumCell = new EntryCell { Label = "Sõnum", Placeholder = "Kirjuta sõnum siia" };

            var nupudView = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(15, 8),
                    Spacing = 8,
                    Children =
                    {
                        TeeBtnNupp("📞 Helista", "#6200EE", Helista_Clicked),
                        TeeBtnNupp("💬 SMS",     "#6200EE", Saada_sms_Clicked),
                        TeeBtnNupp("📧 Email",   "#6200EE", Saada_email_Clicked)
                    }
                }
            };

            var sõnumSection = new TableSection("Saada sõnum") { sõnumCell, nupudView };

            // --- Juhuslik tervitus ---
            tervitusLabel = new Label
            {
                Text = "Vajuta nuppu, et valida juhuslik tervitus",
                FontSize = 13,
                TextColor = Colors.Gray,
                LineBreakMode = LineBreakMode.WordWrap,
                Margin = new Thickness(0, 0, 0, 6)
            };

            var tervitusValikView = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(15, 10),
                    Spacing = 8,
                    Children =
                    {
                        tervitusLabel,
                        TeeBtnNupp("🎲 Vali juhuslik tervitus", "#03DAC6", ValiTervitus_Clicked)
                    }
                }
            };

            var tervitusSaatmineView = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(15, 8),
                    Spacing = 8,
                    Children =
                    {
                        TeeBtnNupp("📱 Tervitus SMS",   "#018786", SaadaTervitusSms_Clicked),
                        TeeBtnNupp("✉️ Tervitus Email", "#018786", SaadaTervitusEmail_Clicked)
                    }
                }
            };

            var tervitusSection = new TableSection("🎉 Juhuslik tervitus")
            {
                tervitusValikView, tervitusSaatmineView
            };

            // --- Pane tabel kokku ---
            tabelview = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot
                {
                    andmedSection,
                    switchSection,
                    fotoSection,
                    sõnumSection,
                    tervitusSection
                }
            };

            Content = tabelview;
        }

        Button TeeBtnNupp(string tekst, string värv, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = Color.FromArgb(värv),
                TextColor = Colors.White,
                CornerRadius = 8,
                FontSize = 12,
                Padding = new Thickness(10, 6)
            };
            btn.Clicked += handler;
            return btn;
        }

        void Sc_OnChanged(object? sender, ToggledEventArgs e)
        {
            if (ic == null || fotoSection == null || sc == null) return;

            if (e.Value)
            {
                fotoSection.Title = "Foto:";
                if (!fotoSection.Contains(ic))
                    fotoSection.Add(ic);
                sc.Text = "Peida foto";
            }
            else
            {
                fotoSection.Title = "";
                fotoSection.Remove(ic);
                sc.Text = "Näita fotot";
            }
        }

        async void Helista_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone))
            {
                await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK");
                return;
            }
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(phone);
            else
                await DisplayAlertAsync("Viga", "Helistamine pole toetatud", "OK");
        }

        async void Saada_sms_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            string message = sõnumCell?.Text ?? "";

            if (string.IsNullOrWhiteSpace(phone))
            {
                await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(message))
                message = "Tere! Saadan sulle sõnumi.";

            SmsMessage sms = new SmsMessage(message, phone);
            if (Sms.Default.IsComposeSupported)
                await Sms.Default.ComposeAsync(sms);
            else
                await DisplayAlertAsync("Viga", "SMS saatmine pole toetatud", "OK");
        }

        async void Saada_email_Clicked(object? sender, EventArgs e)
        {
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            string message = sõnumCell?.Text ?? "";

            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(message))
                message = "Tere tulemast!";

            EmailMessage e_mail = new EmailMessage
            {
                Subject = $"Sõnum sõbrale {nimi}",
                Body = message,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(new[] { email })
            };

            if (Email.Default.IsComposeSupported)
                await Email.Default.ComposeAsync(e_mail);
            else
                await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        void ValiTervitus_Clicked(object? sender, EventArgs e)
        {
            Random rnd = new Random();
            valitudTervitus = tervitused[rnd.Next(tervitused.Count)];
            if (tervitusLabel != null)
                tervitusLabel.Text = $"✅ {valitudTervitus}";
        }

        async void SaadaTervitusSms_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus))
            {
                await DisplayAlertAsync("Viga", "Vali esmalt juhuslik tervitus!", "OK");
                return;
            }
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone))
            {
                await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK");
                return;
            }
            SmsMessage sms = new SmsMessage(valitudTervitus, phone);
            if (Sms.Default.IsComposeSupported)
                await Sms.Default.ComposeAsync(sms);
            else
                await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaTervitusEmail_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus))
            {
                await DisplayAlertAsync("Viga", "Vali esmalt juhuslik tervitus!", "OK");
                return;
            }
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK");
                return;
            }
            EmailMessage e_mail = new EmailMessage
            {
                Subject = $"🎉 Tervitus sõbrale {nimi}",
                Body = valitudTervitus,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(new[] { email })
            };
            if (Email.Default.IsComposeSupported)
                await Email.Default.ComposeAsync(e_mail);
            else
                await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }
    }
}