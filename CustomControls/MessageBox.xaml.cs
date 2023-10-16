using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ECAC_eSports.Methods;

namespace ECAC_eSports.CustomControls
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        public CustomMessageBox() => InitializeComponent();

        public MessageBoxResult Result { get; private set; }

        private readonly Dictionary<MessageBoxImage, ImageSource> _messageBoxImageToSystemIcon = new()
        {
            { MessageBoxImage.Information, GlobalMethods.ImageSourceFromBitmap(Properties.Resources.information)},
            { MessageBoxImage.Exclamation, GlobalMethods.ImageSourceFromBitmap(Properties.Resources.exclamation)},
            { MessageBoxImage.Question, GlobalMethods.ImageSourceFromBitmap(Properties.Resources.question) },
            { MessageBoxImage.Error, GlobalMethods.ImageSourceFromBitmap(Properties.Resources.error) }
        };

        public MessageBoxResult ShowDialog(string description, string title = "ECAC Scraper", MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage messageBoxImage = MessageBoxImage.None)
        {
            return Dispatcher.Invoke(() =>
            {
                mainTitle.Title = title;
                descriptionLabel.Content = description;
                ButtonLeft.Visibility = Visibility.Hidden;
                ButtonMiddle.Visibility = Visibility.Hidden;
                ButtonRight.Visibility = Visibility.Hidden;

                if (messageBoxImage != MessageBoxImage.None)
                {
                    mainTitle.Icon = messageBoxImage switch
                    {
                        MessageBoxImage.Asterisk => _messageBoxImageToSystemIcon[MessageBoxImage.Information],
                        MessageBoxImage.Hand => _messageBoxImageToSystemIcon[MessageBoxImage.Error],
                        _ => _messageBoxImageToSystemIcon[messageBoxImage]
                    };
                }

                switch (messageBoxButton)
                {
                    case MessageBoxButton.OK:
                        ButtonRight.Visibility = Visibility.Visible;
                        ButtonRight.Content = "OK";

                        ButtonRight.Click += delegate
                        {
                            Result = MessageBoxResult.OK;
                            Hide();
                        };

                        break;
                    case MessageBoxButton.OKCancel:
                        ButtonRight.Visibility = Visibility.Visible;
                        ButtonMiddle.Visibility = Visibility.Visible;

                        ButtonRight.Content = "Cancel";
                        ButtonMiddle.Content = "OK";

                        ButtonMiddle.Click += delegate
                        {
                            Result = MessageBoxResult.OK;
                            Hide();
                        };
                        ButtonRight.Click += delegate
                        {
                            Result = MessageBoxResult.Cancel;
                            Hide();
                        };

                        break;
                    case MessageBoxButton.YesNo:
                        ButtonRight.Visibility = Visibility.Visible;
                        ButtonMiddle.Visibility = Visibility.Visible;

                        ButtonRight.Content = "No";
                        ButtonMiddle.Content = "Yes";

                        ButtonMiddle.Click += delegate
                        {
                            Result = MessageBoxResult.Yes;
                            Hide();
                        };
                        ButtonRight.Click += delegate
                        {
                            Result = MessageBoxResult.No;
                            Hide();
                        };

                        break;
                    case MessageBoxButton.YesNoCancel:
                        ButtonRight.Visibility = Visibility.Visible;
                        ButtonMiddle.Visibility = Visibility.Visible;
                        ButtonLeft.Visibility = Visibility.Visible;

                        ButtonRight.Content = "Cancel";
                        ButtonMiddle.Content = "No";
                        ButtonLeft.Content = "Yes";

                        ButtonLeft.Click += delegate
                        {
                            Result = MessageBoxResult.Yes;
                            Hide();
                        };
                        ButtonMiddle.Click += delegate
                        {
                            Result = MessageBoxResult.No;
                            Hide();
                        };
                        ButtonRight.Click += delegate
                        {
                            Result = MessageBoxResult.Cancel;
                            Hide();
                        };

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(messageBoxButton), messageBoxButton, null);
                }

                ShowDialog();
                return Result;
            });
        }
    }
}
