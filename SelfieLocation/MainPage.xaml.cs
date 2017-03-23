using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Media.Capture;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SelfieLocation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Provides functionality to preview and capture the photograph
        private MediaCapture _mediaCapture;

        // This object allows us to manage whether the display goes to sleep 
        // or not while our app is active.
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        // Taken from https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");


        // Tells us if the camera is external or on board.
        private bool _externalCamera = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
           
           //cameraCaptureTask = new CameraCaptureTask();
            //cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

            
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeCameraAsync();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Dispose();
        }
        private async Task InitializeCameraAsync()
        {
            if (_mediaCapture == null)
            {
                // Get the camera devices
                var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // try to get the back facing device for a phone
                var backFacingDevice = cameraDevices
                    .FirstOrDefault(c => c.EnclosureLocation?.Panel == Windows.Devices.Enumeration.Panel.Back);

                // but if that doesn't exist, take the first camera device available
                var preferredDevice = backFacingDevice ?? cameraDevices.FirstOrDefault();

                // Store whether the camera is onboard of if it's external.
                _externalCamera = backFacingDevice == null;

                // Create MediaCapture
                _mediaCapture = new MediaCapture();

                // Stop the screen from timing out.
                _displayRequest.RequestActive();

                // Initialize MediaCapture and settings
                await _mediaCapture.InitializeAsync(
                    new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = preferredDevice.Id
                    });

                // Set the preview source for the CaptureElement
                PreviewControl.Source = _mediaCapture;

                // Start viewing through the CaptureElement 
                await _mediaCapture.StartPreviewAsync();

                // Set rotation properties to ensure the screen is filled with the preview.
                await SetPreviewRotationPropertiesAsync();
            }
        }

        private async void snapButton_Click(object sender, RoutedEventArgs e)
        { 
            // This is where we want to save to.
            var storageFolder = KnownFolders.SavedPictures;

            // Create the file that we're going to save the photo to.
            var file = await storageFolder.CreateFileAsync("sample.jpg", CreationCollisionOption.ReplaceExisting);

            // Update the file with the contents of the photograph.
            await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);
        }

        private void Dispose()
        {
            if (_mediaCapture != null)
            {
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }

            if (PreviewControl.Source != null)
            {
                PreviewControl.Source.Dispose();
                PreviewControl.Source = null;
            }

            _displayRequest.RequestRelease();
        }

        private async Task SetPreviewRotationPropertiesAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (_externalCamera) return;

            // Calculate which way and how far to rotate the preview
            int rotation = ConvertDisplayOrientationToDegrees(DisplayInformation.GetForCurrentView().CurrentOrientation);

            // Get the property meta data about the video.
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);

            // Change the meta data to rotate the preview to fill the screen with the preview.
            props.Properties.Add(RotationKey, rotation);

            // Now set the updated meta data into the video preview.
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        // Taken from https://msdn.microsoft.com/en-us/windows/uwp/audio-video-camera/capture-photos-and-video-with-mediacapture
        private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }

        private async Task ComposeEmail(Windows.ApplicationModel.Contacts.Contact recipient,
    string messageBody,
    StorageFile attachmentFile)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Body = messageBody;

            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                    attachmentFile.Name,
                    stream);

                emailMessage.Attachments.Add(attachment);
            }

            var email = recipient.Emails.FirstOrDefault<Windows.ApplicationModel.Contacts.ContactEmail>();
            if (email != null)
            {
                var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(email.Address);
                emailMessage.To.Add(emailRecipient);
            }

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            // This is where we want to save to.
            //StorageFolder storageFolder =KnownFolders.SavedPictures;
            //StorageFile photoFile = await storageFolder.GetFileAsync("sample.jpg");
            StorageFolder storageFolder = Windows.A
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.SelectionMode = Windows.ApplicationModel.Contacts.ContactSelectionMode.Fields;
            contactPicker.DesiredFieldsWithContactFieldType.Add(Windows.ApplicationModel.Contacts.ContactFieldType.Email);
            Windows.ApplicationModel.Contacts.Contact recipient = await contactPicker.PickContactAsync();

            await ComposeEmail(recipient, chatBox.Text, photoFile);

        }

        
    }
}
