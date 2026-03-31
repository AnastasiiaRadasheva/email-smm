using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace _6osa
{
    public partial class ContactPage : ContentPage
    {
        Entry? nimeEntry;
        Entry? emailEntry;
        Entry? telefonEntry;
        Entry? kirjeldusEntry;
        Entry? sõnumEntry;
        Image? fotoImage;
        Label? tervitusLabel;
        Picker? tervitusPicker;

        readonly Color põhiVärv = Color.FromArgb("#6C63FF");
        readonly Color teineVärv = Color.FromArgb("#FF6584");
        readonly Color roheVärv = Color.FromArgb("#43C59E");
        readonly Color taustaVärv = Color.FromArgb("#F4F3FF");
        readonly Color kaartVärv = Colors.White;
        readonly Color tekstVärv = Color.FromArgb("#2D2B55");
        readonly Color hallVärv = Color.FromArgb("#9B99B5");

        bool fotoNähtav = false;
        string valitudTervitus = "";

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

        public ContactPage()
        {
            Title = "Kontaktiraamat";
            BackgroundColor = taustaVärv;
            EhitaLeht();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () =>
            {
                nimeEntry?.Focus();
            });
        }

        void EhitaLeht()
        {
            var leht = new VerticalStackLayout
            {
                Padding = new Thickness(16, 12),
                Spacing = 16
            };

            leht.Add(new Label
            {
                Text = "👥 Sõbrade kontaktiraamat",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = tekstVärv,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 8, 0, 0)
            });

            nimeEntry = TeeEntry("Sõbra nimi", "👤", Keyboard.Default);
            emailEntry = TeeEntry("Email", "📧", Keyboard.Email);
            telefonEntry = TeeEntry("Telefoninumber", "📞", Keyboard.Telephone);
            kirjeldusEntry = TeeEntry("Kirjeldus sõbra kohta", "📝", Keyboard.Default);

            leht.Add(TeeKaart("👤 Sõbra andmed", põhiVärv, new VerticalStackLayout
            {
                Spacing = 10,
                Children = { nimeEntry, emailEntry, telefonEntry, kirjeldusEntry }
            }));

            fotoImage = new Image
            {
                Source = ImageSource.FromFile("dotnet_bot.png"),
                HeightRequest = 160,
                Aspect = Aspect.AspectFill,
                IsVisible = false
            };

            var fotoToggle = new Switch
            {
                IsToggled = false,
                OnColor = põhiVärv
            };
            var fotoToggleLabel = new Label
            {
                Text = "Näita fotot",
                TextColor = tekstVärv,
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center
            };
            fotoToggle.Toggled += (s, e) =>
            {
                fotoNähtav = e.Value;
                fotoImage.IsVisible = fotoNähtav;
                fotoToggleLabel.Text = fotoNähtav ? "Peida foto" : "Näita fotot";
            };

            var toggleRida = new HorizontalStackLayout
            {
                Spacing = 10,
                Children = { fotoToggleLabel, fotoToggle }
            };

            var fotoNupud = new HorizontalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    TeeNupp(" Teen foto",  põhiVärv, TeeFoto_Clicked),
                    TeeNupp(" Valin foto", põhiVärv, ValiGaleriist_Clicked)
                }
            };

            leht.Add(TeeKaart(" Foto", põhiVärv, new VerticalStackLayout
            {
                Spacing = 10,
                Children = { toggleRida, fotoNupud, fotoImage }
            }));

            // ── KAART: Saada sõnum ────────────────────────────
            sõnumEntry = TeeEntry("Kirjuta sõnum siia...", "💬", Keyboard.Default);

            var sõnumNupud = new FlexLayout
            {
                Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
                AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
                Direction = Microsoft.Maui.Layouts.FlexDirection.Row
            };
            sõnumNupud.Add(TeeNuppMargin(" Helista", põhiVärv, Helista_Clicked));
            sõnumNupud.Add(TeeNuppMargin(" SMS", põhiVärv, Saada_sms_Clicked));
            sõnumNupud.Add(TeeNuppMargin("Email", põhiVärv, Saada_email_Clicked));

            leht.Add(TeeKaart(" Saada sõnum", põhiVärv, new VerticalStackLayout
            {
                Spacing = 10,
                Children = { sõnumEntry, sõnumNupud }
            }));

            // ── KAART: Juhuslik tervitus ──────────────────────
            tervitusLabel = new Label
            {
                Text = "Vali tervitus nimekirjast või vajuta juhuslik 🎲",
                FontSize = 13,
                TextColor = hallVärv,
                LineBreakMode = LineBreakMode.WordWrap
            };

            tervitusPicker = new Picker
            {
                Title = " Vali tervitus...",
                TextColor = tekstVärv,
                TitleColor = hallVärv,
                BackgroundColor = taustaVärv
            };
            foreach (var t in tervitused)
                tervitusPicker.Items.Add(t);
            tervitusPicker.SelectedIndexChanged += (s, e) =>
            {
                if (tervitusPicker.SelectedIndex >= 0)
                {
                    valitudTervitus = tervitused[tervitusPicker.SelectedIndex];
                    if (tervitusLabel != null)
                    {
                        tervitusLabel.Text = $"✅ {valitudTervitus}";
                        tervitusLabel.TextColor = roheVärv;
                    }
                }
            };

            var pickerBorder = new Border
            {
                Stroke = põhiVärv,
                StrokeThickness = 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                Padding = new Thickness(4, 0),
                Content = tervitusPicker
            };

            var tervitusNupud = new FlexLayout
            {
                Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Start,
                Direction = Microsoft.Maui.Layouts.FlexDirection.Row
            };
            tervitusNupud.Add(TeeNuppMargin("🎲 Juhuslik", roheVärv, ValiTervitus_Clicked));
            tervitusNupud.Add(TeeNuppMargin("📱 Tervitus SMS", teineVärv, SaadaTervitusSms_Clicked));
            tervitusNupud.Add(TeeNuppMargin("✉️ Tervitus Email", teineVärv, SaadaTervitusEmail_Clicked));

            leht.Add(TeeKaart("🎉 Juhuslik tervitus", roheVärv, new VerticalStackLayout
            {
                Spacing = 12,
                Children = { tervitusLabel, pickerBorder, tervitusNupud }
            }));

            leht.Add(new BoxView { HeightRequest = 20, Color = Colors.Transparent });

            Content = new ScrollView { Content = leht };
        }

        Border TeeKaart(string pealkiri, Color aktsent, View sisu)
        {
            var pealkirjaLabel = new Label
            {
                Text = pealkiri,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = aktsent,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var sisu2 = new VerticalStackLayout
            {
                Spacing = 0,
                Children = { pealkirjaLabel, sisu }
            };

            return new Border
            {
                BackgroundColor = kaartVärv,
                Stroke = Color.FromArgb("#E8E6FF"),
                StrokeThickness = 1.5,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) },
                Padding = new Thickness(16, 14),
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Color.FromArgb("#6C63FF")),
                    Offset = new Point(0, 4),
                    Radius = 12,
                    Opacity = 0.08f
                },
                Content = sisu2
            };
        }

        Entry TeeEntry(string placeholder, string ikoon, Keyboard keyboard)
        {
            return new Entry
            {
                Placeholder = $"{ikoon}  {placeholder}",
                PlaceholderColor = hallVärv,
                TextColor = tekstVärv,
                BackgroundColor = taustaVärv,
                Keyboard = keyboard,
                FontSize = 14,
                MinimumHeightRequest = 44
            };
        }

        Button TeeNupp(string tekst, Color värv, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = värv,
                TextColor = Colors.White,
                CornerRadius = 10,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(14, 10),
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(värv),
                    Offset = new Point(0, 3),
                    Radius = 8,
                    Opacity = 0.3f
                }
            };
            btn.Clicked += handler;
            return btn;
        }

        View TeeNuppMargin(string tekst, Color värv, EventHandler handler)
        {
            var btn = TeeNupp(tekst, värv, handler);
            btn.Margin = new Thickness(0, 4, 8, 4);
            return btn;
        }

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

                string newFile = System.IO.Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = System.IO.File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                if (fotoImage != null)
                {
                    fotoImage.Source = ImageSource.FromFile(newFile);
                    fotoImage.IsVisible = true;
                    fotoNähtav = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", ex.Message, "OK");
            }
        }

        async void ValiGaleriist_Clicked(object? sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo == null) return;

                if (fotoImage != null)
                {
                    fotoImage.Source = ImageSource.FromFile(photo.FullPath);
                    fotoImage.IsVisible = true;
                    fotoNähtav = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", ex.Message, "OK");
            }
        }

        async void Helista_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonEntry?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(phone);
            else await DisplayAlertAsync("Viga", "Helistamine pole toetatud", "OK");
        }

        async void Saada_sms_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonEntry?.Text ?? "";
            string message = sõnumEntry?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (string.IsNullOrWhiteSpace(message)) message = "Tere! Saadan sulle sõnumi.";
            var sms = new SmsMessage(message, phone);
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(sms);
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void Saada_email_Clicked(object? sender, EventArgs e)
        {
            string email = emailEntry?.Text ?? "";
            string nimi = nimeEntry?.Text ?? "Sõber";
            string message = sõnumEntry?.Text ?? "";
            if (string.IsNullOrWhiteSpace(email)) { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }
            if (string.IsNullOrWhiteSpace(message)) message = "Tere tulemast!";
            var mail = new EmailMessage
            {
                Subject = $"Sõnum sõbrale {nimi}",
                Body = message,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(new[] { email })
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        void ValiTervitus_Clicked(object? sender, EventArgs e)
        {
            var rnd = new Random();
            valitudTervitus = tervitused[rnd.Next(tervitused.Count)];
            if (tervitusLabel != null)
            {
                tervitusLabel.Text = $"✅ {valitudTervitus}";
                tervitusLabel.TextColor = roheVärv;
            }
            int idx = tervitused.IndexOf(valitudTervitus);
            if (tervitusPicker != null && idx >= 0)
                tervitusPicker.SelectedIndex = idx;
        }

        async void SaadaTervitusSms_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string phone = telefonEntry?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            var sms = new SmsMessage(valitudTervitus, phone);
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(sms);
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaTervitusEmail_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string email = emailEntry?.Text ?? "";
            string nimi = nimeEntry?.Text ?? "Sõber";
            if (string.IsNullOrWhiteSpace(email)) { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }
            var mail = new EmailMessage
            {
                Subject = $"🎉 Tervitus sõbrale {nimi}",
                Body = valitudTervitus,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(new[] { email })
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }
    }
}