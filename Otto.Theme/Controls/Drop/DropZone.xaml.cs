using Otto.Theme.Tools.Extension;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Otto.Theme.Controls
{
    /// <summary>
    /// Logique d'interaction pour DropZone.xaml
    /// </summary>
    public partial class DropZone
    {
        #region Events

        public event EventHandler<string[]> OnDisallowedFileExtensionsDragEnter;

        public event EventHandler<string[]> OnDisallowedFileExtensionsDragLeave;

        public event EventHandler<string[]> OnAllowedFileExtensionsDraggedEnter;

        public event EventHandler<string[]> OnAllowedFileExtensionsDraggedLeave;

        #endregion Events

        #region Text

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(DropZone), new PropertyMetadata("Click or drop files here"));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion Text

        #region TextWhenAllowedExtension

        public static readonly DependencyProperty TextWhenAllowedExtensionProperty =
           DependencyProperty.Register("TextWhenAllowedExtension", typeof(string), typeof(DropZone), new PropertyMetadata(null));

        public string TextWhenAllowedExtension
        {
            get { return (string)GetValue(TextWhenAllowedExtensionProperty); }
            set { SetValue(TextWhenAllowedExtensionProperty, value); }
        }

        #endregion TextWhenAllowedExtension

        #region TextWhenDisallowedExtension

        public static readonly DependencyProperty TextWhenDisallowedExtensionProperty =
           DependencyProperty.Register("TextWhenDisallowedExtension", typeof(string), typeof(DropZone), new PropertyMetadata(null));

        public string TextWhenDisallowedExtension
        {
            get { return (string)GetValue(TextWhenDisallowedExtensionProperty); }
            set { SetValue(TextWhenDisallowedExtensionProperty, value); }
        }

        #endregion TextWhenDisallowedExtension

        #region TextForeground

        public static readonly DependencyProperty TextForegroundProperty =
           DependencyProperty.Register("TextForeground", typeof(Brush), typeof(DropZone), new PropertyMetadata(Brushes.DarkGray));

        public Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        #endregion TextForeground

        #region IsDragging

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(DropZone), new UIPropertyMetadata(false));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set
            {
                SetValue(IsDraggingProperty, value);
                Console.WriteLine("IsDragging: " + value);
            }
        }

        #endregion IsDragging

        #region DraggedFilesHaveAllowedExtensions

        public static readonly DependencyProperty DraggedFilesHaveAllowedExtensionsProperty =
            DependencyProperty.RegisterAttached("DraggedFilesHaveAllowedExtensions", typeof(bool), typeof(DropZone), new UIPropertyMetadata(true));

        public bool DraggedFilesHaveAllowedExtensions
        {
            get { return (bool)GetValue(DraggedFilesHaveAllowedExtensionsProperty); }
            set
            {
                SetValue(DraggedFilesHaveAllowedExtensionsProperty, value);
                Console.WriteLine("DraggedFilesHaveAllowedExtensions: " + value);
            }
        }

        #endregion DraggedFilesHaveAllowedExtensions

        #region AllowedExtensions

        public static readonly DependencyProperty AllowedExtensionsProperty =
           DependencyProperty.Register("AllowedExtensions", typeof(string), typeof(DropZone), new PropertyMetadata("*"));

        public string AllowedExtensions
        {
            get { return (string)GetValue(AllowedExtensionsProperty); }
            set { SetValue(AllowedExtensionsProperty, value); }
        }

        #endregion AllowedExtensions

        #region Private

        private string[] DraggedFiles { get; set; }
        private bool AreDraggedFilesAllowed { get; set; }

        #endregion Private

        public DropZone()
        {
            InitializeComponent();
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                source.SetParentValue<Button>(IsDraggingProperty, false);
                source.SetParentValue<Button>(DraggedFilesHaveAllowedExtensionsProperty, true);

                if (AreDraggedFilesAllowed)
                    OnAllowedFileExtensionsDraggedLeave?.Invoke(this, DraggedFiles);
                else
                    OnDisallowedFileExtensionsDragLeave?.Invoke(this, DraggedFiles);
            }
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            var source = e.OriginalSource as DependencyObject;
            source.SetParentValue<Button>(IsDraggingProperty, true);
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                DraggedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                AreDraggedFilesAllowed = DoDraggedFilesHaveAllowedExtensions(DraggedFiles);

                source.SetParentValue<Button>(DraggedFilesHaveAllowedExtensionsProperty, AreDraggedFilesAllowed);

                if (AreDraggedFilesAllowed)
                    OnAllowedFileExtensionsDraggedEnter?.Invoke(this, DraggedFiles);
                else
                    OnDisallowedFileExtensionsDragEnter?.Invoke(this, DraggedFiles);
            }
            else
            {
                source.SetParentValue<Button>(DraggedFilesHaveAllowedExtensionsProperty, false);

                OnDisallowedFileExtensionsDragEnter?.Invoke(this, DraggedFiles);
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                source.SetParentValue<Button>(IsDraggingProperty, false);
                source.SetParentValue<Button>(DraggedFilesHaveAllowedExtensionsProperty, true);
            }

            base.OnDrop(e);
        }

        internal bool DoDraggedFilesHaveAllowedExtensions(string[] fileNames)
        {
            if (AllowedExtensions.ToLower().Contains("*"))
                return true;

            foreach (string filename in fileNames)
            {
                // If at leats one of the current dragged files extension is not found in the allowed extensions array, we return false
                if (AllowedExtensions.ToLower().Contains(System.IO.Path.GetExtension(filename).ToLower()) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}