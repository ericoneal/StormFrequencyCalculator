   M S B A M L   `   `   `  ���� P  LAvalonControlsLibrary, Version=2.0.0.0, Culture=neutral, PublicKeyToken=nulltNclr-namespace:AC.AvalonControlsLibrary.Controls;assembly=AvalonControlsLibrary!AC.AvalonControlsLibrary.Controls   StormFrequencyCalculator.   (StormFrequencyCalculator.WPFStormControl   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    >d2http://schemas.microsoft.com/expression/blend/2008    ?toolkit-http://schemas.microsoft.com/wpf/2008/toolkit    [myNclr-namespace:AC.AvalonControlsLibrary.Controls;assembly=AvalonControlsLibrary    .���   � 	Resources     DarkBackground �� 
StartPoint �� EndPoint � Offset � Color  GlassFX  GlassFXDisabled 
 GlowFX 
� GradientOrigin 
� Center  GlowFXPressed  GlowFXRowPressed  GlowFXProgress  GlowFXProgressAnimated � MappingMode  GlowFXTabSelected 	 GlowOut W�Name	 g� 	BeginTime
 �� 
TargetName g� Duration W� From	 W� To �� TargetProperty 

 GlowIn  ButtonFocusVisual �� Property �� Value J� SnapsToDevicePixels �� StrokeDashArray  RoundedButton �� 
TargetType �� 
TargetType � 	Resources  Key O� KeyTime O� Value ��Name �� CornerRadius J� RenderTransformOrigin �� ScaleX �� ScaleY �� AngleX �� AngleY �� Angle  T� X! T� Y" � RowDefinitions# J� Opacity$ ��Name% �� Triggers& P� Property' P� Value( �� 
TargetName) N� ExitActions* ��Name  	HideShine+ N� EnterActions  	ShowShine��%�
  &	        &	 V     &	 �     &	     &	 ~    &	 �    &	 8    &	 �    &	     &		 n    &	
 �    &	 Q    &	 �    7  7  7  �� $	 0,0&�$	 1,1&�.��� $ 0Q�$ 	#FF333344��� $ 1Q�$ 	#FF666677���� $
 .5,0&�$ .5,.5&�.��� $ 1Q�$ 	#33DDDDDD��� $ 1Q�$ 	#33000000���� $
 .5,0&�$ .5,.5&�.��� $ 1Q�$ 	#33BBBBBB��� $ 1Q�$ 	#11000000��
� $
 .5,1&�$
 .5,1&�.��� $ 0Q�$ 	#990000FF��� $ .5Q�$ 	#660000DD��� $ 1Q�$ 	#33000000��
� $
 .5,1&�$
 .5,1&�.��� $ 0Q�$ 	#660000CC��� $	 1.2Q�$ 	#33FFFFFF���� .��� $ 0Q�$ 	#660000FF��� $ .7Q�$ 	#660000AA��� $ 1Q�$ 	#66000000���� .��� $ 0Q�$ 	#660099FF��� $	 .99Q�$ 	#660022AA��� $ 1Q�$ 	#00000000���� $ RelativeToBoundingBox=�.��� $ 0Q�$ 	#00000000��� $	 .50Q�$ 	#660099FF��� $ 1Q�$ 	#00000000���� $
 .5,1&�$
 .5,0&�.��� $ 0Q�$ 	#33DDDDDD��� $ 1Q�$ 	#332222FF���� .��W� $ AnimGlowOut��$	 00:00:00X�$
 GlowRectangle��$ 00:00:00.250D�$ 1X�$ 0X�$ Opacity��� .��W� $ AnimGlow��$	 00:00:00X�$
 GlowRectangle��$ 00:00:00.250D�$ 0X�$ 1X�$ Opacity��� .���  � �� �� .R��� ._���  . $��4q�
���   �$	 1 2R�$��1���� " ��.���  �@�FocusVisualStyle8   ��  � ��# � ����  � ��# � ����  � ��$ 3q���  � ��$ Center=���  � ��$ Center=���  � ��$ 0,0,1,1q���  � �� �� " �� ���� 	ShowShine .��U� $	 00:00:00X�$
 Shine��$ (UIElement.Opacity)�.\��� $ 00:00:00.5000000��$ 1Q��� 	HideShine .��U� $	 00:00:00X�$
 Shine��$ (UIElement.Opacity)�.\��� $ 00:00:00.3000000��$ 0Q�.R��� $ border��$ 5,5,5,5��$��1,1,1,1q�$ 0.5,0.5&����	#FFFFFFFFz�V� .~��� $ 1Q�$ 1Q��� $ 0Q�$ 0Q��� $ 0Q�T� $  0Q�$! 0Q�._��� $ border1��#��z��$ 5,5,5,5��._�� " �� $
��0.5* ��� $
��0.5* �.+��� $��0��$ 5,5,0,0������ $ 0.5,1&�$ 0.5,0&�.��� $ 	#00FFFFFF��$ 0Q�� $ 	#7EFFFFFF��$ 1Q��� $ Shine��$��1��$# 0Q�$
��Auto��$
��Auto��$ 0,0,5,5��$��1,0,-1,0q�#��z���� $$ contentPresenter��$��Center=�$��2��$��Center=�% P� & � {�' .  . ��� $( border1�� �@J�Opacity$	 0.5Q��� $( border�� �@J�Opacity$ 1Q��� $( contentPresenter�� �@J�Opacity$	 0.5Q�P� & � ��' . . ��� $( border�� � z� V� .~��� $	 0.9Q�$	 0.9Q��� $ 0Q�$ 0Q��� $ 0Q�T� $  0Q�$! 0Q�P� & �@J�IsMouseOver' . ) �� $* HideShine_BeginStoryboard��8�� + �� $* ShowShine_BeginStoryboard��8�� ���� $	 0,0&�$	 1,1&�.��� $ 0Q�$ 	#FF333344��� $ 1Q�$ 	#FF666677��.+��� .+��� $��10,10,10,10q�$��12(�$
��Bold"�$��Layer Settings��
�������$��2q����	#FF070101��Z� .���� , �� Orientation$, 
Horizontal=�$��Center=�.+��� $��Right=�.+��� $��Input (Point) Layer:��
��������� $��  Select Label Field:��$��0,10,0,0q�
��������� $
��Left=�.+��� -   $��cboPointLayer��$	��200���� -   $��cboLayerFields��$��0,10,0,0q��� .+��� $��10,10,10,10q�$��12(�$
��Bold"�$��Time Settings��
�������$��2q����	#FF070101��Z� .���� $, 
Horizontal=�.+��� $
��Left=�.+��� $, 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� $��Start Date:��
�������2   ,AC.AvalonControlsLibrary.Controls.DatePicker  -   $��dtStart��$	��197���� $, 
Horizontal=�$��Center=�$��0,20,0,0q�.+��� $��  End Date:��
�������  -   $��dtEnd��$	��197���� $, 
Horizontal=�$��Center=�$��0,20,0,0q�.+��� $�� Rain Data:��
��������� -   $��cboRainData��$	��153��.@��� $��	Real Time��- �� 
IsSelected- . �� $��Corrected (EOM)���� $��
Rain Guage���� $, 
Horizontal=�$��Center=�$��0,20,0,0q�.+��� -   $��chkUseBatch��$�� ��$��10,5,0,0q�
���   ��� $��Batch Process Dates��
�������$��0,0,10,0q��� -   $��btnDateHelp��#��[ $��20��$��20��#���  ���	#83FFFFFF
�������$��12(�.��?�� $, Vertical=�$��	0,10,10,0q�.+��� -   $��txtStart��$��	10,00,0,0q�$��5/1/2012���� -	   $��txtEnd��$��	10,20,0,0q�$	��120���� -
   $��btnCalculate��#��[ $��30��$	��100��#���  ���	#83FFFFFF
�������$��14(�$��	0,25,10,0q�.��	Calculate