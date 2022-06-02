using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace SimpleTextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isTextEditorChanged = false;
        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            this.Title = "untitled";
            
        }

        private void Open_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Word Document (*.doc)|*.doc|Text File (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                TextRange range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
                String dataFormats = GetDataFormatByExtension(dlg.FileName);
                range.Load(fileStream, dataFormats);
                fileStream.Close();
                this.Title = dlg.FileName;
            }
        }

        private void Save_File(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|Word Document (*.doc)|*.doc|Text File (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
                String dataFormats = GetDataFormatByExtension(dlg.FileName);
                range.Save(fileStream, dataFormats);
                fileStream.Close();
                this.Title = dlg.FileName;
            }
        }


        private void RctSelectionChanged(object sender, RoutedEventArgs e)
        {
            isTextEditorChanged = true;
            object temp = textEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = textEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = textEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));
            //alignment
            btnAlignLeft.IsChecked = false;
            btnAlignCenter.IsChecked = false;
            btnAlignRight.IsChecked = false;
            temp = textEditor.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
            btnAlignLeft.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Left));
            btnAlignCenter.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Center));
            btnAlignRight.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextAlignment.Right));
            
            temp = textEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = textEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            cmbFontSize.SelectedItem = temp;

            //color
            temp = textEditor.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if(temp != DependencyProperty.UnsetValue)
                colorPicker.SelectedColor = ((SolidColorBrush)temp).Color;

            isTextEditorChanged = false;
        }

        private void SelectionChanged_fontFamily(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null && isTextEditorChanged == false)
            {
                textEditor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, cmbFontFamily.SelectedItem);
                cmbFontFamily.Focusable = false;
            }
        }

        private void SelectionChange_fontSize(object sender, TextChangedEventArgs e)
        {
            if(cmbFontSize.SelectedItem != null && isTextEditorChanged == false) { 
                textEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cmbFontSize.Text);
                cmbFontSize.Focusable = false;
            }
        }

        private void AlignmentButtonClickHandler(object sender, RoutedEventArgs e)
        {
            string btn = ((System.Windows.Controls.Primitives.ToggleButton)sender).Name;
            if(btn == "btnAlignLeft")
            {
                btnAlignCenter.IsChecked = false;
                btnAlignRight.IsChecked = false;
            }
            if (btn == "btnAlignCenter")
            {
                btnAlignLeft.IsChecked = false;
                btnAlignRight.IsChecked = false;
            }
            if (btn == "btnAlignRight")
            {
                btnAlignCenter.IsChecked = false;
                btnAlignLeft.IsChecked = false;
            }
        }

        private String GetDataFormatByExtension(String filename)
        {
            string ext = System.IO.Path.GetExtension(filename);
            switch (ext)
            {
                case ".rtf":
                case ".doc":
                    return DataFormats.Rtf;
                case ".txt":
                    return DataFormats.Text;
            }

            return null;
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(isTextEditorChanged == false) { 
                Color color = (Color)ColorConverter.ConvertFromString(colorPicker.SelectedColor.ToString());
                textEditor.Selection.ApplyPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty, new SolidColorBrush(color));
                colorPicker.Focusable = false;
            }
        }

    }
}
