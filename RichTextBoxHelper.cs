using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace tcp_auto
{
	public class RichTextBoxHelper : DependencyObject
	{
		public static string GetRichText(DependencyObject obj)
		{
			return (string)obj.GetValue(RichTextProperty);
		}
		public static void SetRichText(DependencyObject obj, string value)
		{
			obj.SetValue(RichTextProperty, value);
		}
		public static readonly DependencyProperty RichTextProperty =
			DependencyProperty.RegisterAttached("RichText", typeof(string), typeof(RichTextBoxHelper), new FrameworkPropertyMetadata
			{
				BindsTwoWayByDefault = true,
				PropertyChangedCallback = (obj, e) =>
				{
					var richTextBox = (RichTextBox)obj;
					var text = GetRichText(richTextBox);
					richTextBox.AppendText(text);
					richTextBox.AppendText(Environment.NewLine);
					richTextBox.ScrollToEnd();
				}
			});
	}
}
