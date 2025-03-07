# -------------
# App.xaml
# -------------

$header = @"
<Application x:Class="ActivAndZen.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
			 xmlns:local="clr-namespace:ActivAndZen.Components">
    <Application.Resources>
		<ResourceDictionary>
			<ResourceDictionary.MergedDictionaries>
"@
			
$footer = @"
			</ResourceDictionary.MergedDictionaries>
		</ResourceDictionary>
	</Application.Resources>
</Application>
"@

cd .\src\Assets\xaml\
$files = ls_files -split "`n"
cd ..\..\..

$content

foreach( $f in $files) {
	$content = $content + "`t`t`t`t" + "<ResourceDictionary Source=`"src/Assets/xaml/" + $f + "`"/>`r`n"
}

$appxaml = $header + "`r`n" + $content + $footer

Set-Content .\App.xaml $appxaml

# -------------
# Ressources.cs
# -------------

# (DrawingBrush)Application.Current.Resources[iconId]

$header = @"
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace ActivAndZen;

public partial class App {
	public static class Icons {
"@

$footer = @"
	}
}
"@

$content = ""

foreach( $f in $files) {
	$icon = $f.Substring(0, $f.Length - 5) 
	$content = $content + "`t`t" + "public static DrawingBrush " + $icon.Substring(0, $icon.Length - 4)
	$content = $content + " = (DrawingBrush)Application.Current.Resources[`"db_"  + $icon + "`"];`r`n" 
}

$ressources = $header + "`r`n" + $content + $footer

Set-Content .\AppResources.cs $ressources