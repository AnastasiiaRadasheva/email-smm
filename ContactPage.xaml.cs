using Microsoft.Maui.Controls;

namespace _6osa
{
    // Ainult .cs fail - XAML faili pole vaja, kustuta ContactPage.xaml kui see on olemas
    public partial class ContactPage : ContentPage
    {
        // --- Kõik elemendid ---
        Entry? fookusEntry; // peidetud Entry klaviatuuri avamiseks
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

        // Klaviatuur avaneb automaatselt kui leht ilmub
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () =>
            {
                fookusEntry?.Focus();
            });
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

            // --- Foto nupud (kaamera ja galerii) ---
            var fotoNupudView = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(15, 8),
                    Spacing = 8,
                    Children =
                    {
                        TeeBtnNupp("📷 Teen foto",  "#6200EE", TeeFoto_Clicked),
                        TeeBtnNupp("🖼️ Valin foto", "#6200EE", ValiGaleriist_Clicked)
                    }
                }
            };

            var switchSection = new TableSection("Foto") { sc, fotoNupudView };
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
                Margin = new Thickness(0, 0, 0, 4)
            };

            // Picker - vali ise konkreetne tervitus
            var tervitusPicker = new Picker
            {
                Title = "📋 Vali tervitus nimekirjast...",
                HorizontalOptions = LayoutOptions.Fill
            };
            foreach (var t in tervitused)
                tervitusPicker.Items.Add(t);
            tervitusPicker.SelectedIndexChanged += (s, e) =>
            {
                if (tervitusPicker.SelectedIndex >= 0)
                {
                    valitudTervitus = tervitused[tervitusPicker.SelectedIndex];
                    tervitusLabel.Text = $"✅ {valitudTervitus}";
                }
            };

            // Rändomi nupp
            var rndNupp = TeeBtnNupp("🎲 Juhuslik", "#03DAC6", ValiTervitus_Clicked);
            rndNupp.HorizontalOptions = LayoutOptions.Fill;

            var tervitusValikView = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(15, 10),
                    Spacing = 6,
                    Children =
                    {
                        tervitusLabel,
                        tervitusPicker,
                        rndNupp
                    }
                }
            };
            tervitusValikView.Height = 160;

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

            // Peidetud Entry klaviatuuri automaatseks avamiseks
            fookusEntry = new Entry
            {
                IsVisible = false,
                WidthRequest = 1,
                HeightRequest = 1
            };

            Content = new Grid
            {
                Children = { tabelview, fookusEntry }
            };
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

        // --- KAAMERA: teen foto ---
        async void TeeFoto_Clicked(object? sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlertAsync("Viga", "Kaamera pole toetatud sellel seadmel", "OK");
                    return;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null) return;

                // Salvestame foto rakenduse andmekausta
                string newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                // Uuendame ImageCell pildi
                if (ic != null)
                {
                    ic.ImageSource = ImageSource.FromFile(newFile);
                    ic.Detail = "Tehtud foto";
                }

                // Kui switch on väljas, lülitame sisse automaatselt
                if (sc != null && !sc.On)
                    sc.On = true;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", ex.Message, "OK");
            }
        }

        // --- GALERII: valin foto ---
        async void ValiGaleriist_Clicked(object? sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo == null) return;

                // Laeme pildi ImageCell-i
                if (ic != null)
                {
                    ic.ImageSource = ImageSource.FromFile(photo.FullPath);
                    ic.Detail = "Valitud galeriist";
                }

                // Kui switch on väljas, lülitame sisse automaatselt
                if (sc != null && !sc.On)
                    sc.On = true;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", ex.Message, "OK");
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