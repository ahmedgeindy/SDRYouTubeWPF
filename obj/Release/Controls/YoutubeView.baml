   M S B A M L   `   `   `  ���� `  \Genesyslab.Desktop.Modules.Sdr.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common.Genesyslab.Desktop.Modules.Sdr.Common.Controls  g cGenesyslab.Desktop.Modules.YoutubeWorkItem, Version=8.5.122.8, Culture=neutral, PublicKeyToken=nullyAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls3Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls E   ?Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.YoutubeView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ~commonkclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common     VControlsAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls       � 	Resources   �� MergedDictionaries �� Source    BooleanToVisibilityConverter��  �� $r lpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Resources/YoutubeMainStyles.xaml?�%   &	        �� .��� -    �Name$ myGrid��$��1q�$��Stretch=�
������� � RowDefinitions �� $��40 ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+�;   5Genesyslab.Desktop.Modules.Sdr.Common.Controls.Header  $��0��$��0��$��2��$��1q�
�������   HeaderImageSource$h bpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Images/YoutubeLogo.png��>  8Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Post  -     Name$ PostControl��$��0��$��1��$	��550��$
��Left=� � DataContext �� *Post+A  ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment  -     Name$ 	InComment��A  ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment	   Loaded$	 InComment_Loaded��$��2��$��0��$	��550��$
��Left=�$	��6 0q�
   SendButtonVisibility$
 	Collapsed=� �� *	InComment+�� -   $��
icTodoList��$��3��$��0��$��
29 10 10 0q�$
��Left=�$
��Auto��
������ ���� *ReplyMessages+���� .R�     	SendClick OutComment_SendClick   DeleteClick InComment_DeleteClick$	��521��$
��Left=�$	��6 0q�   ReplyButtonVisibility$ 	Collapsed=�   LikeButtonVisibility$ 	Collapsed=�   DeleteButtonVisibility �� *Comment.DeleteButtonVisibilty+
 �� *SendButtonVisibility+�� -   $��OutCommentsList��$��4��$��0��$��	0 10 10 0q�$
��Left=�$
��Auto��
������ ���� *OutComments+���� .R�   OutComment_SendClick InComment_DeleteClick$	��550��$
��Left=�$	��6 0q�$ 	Collapsed=�$ 	Collapsed=�
 �� *SendButtonVisibility+ �� *DeleteButtonVisibilty+D  >Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.AddComment  -     Name$ AddCommentControl��$��5��$��Center=�$
��Left=�$y�	Collapsed=� �� *
AddComment+� -   $ LoadingOverlay��$��0��$��6�����	#40FFFFFF$y�	Collapsed=�z��� -   LoadingScale�� �� ScaleX$	 0.3Q� �� ScaleY$	 0.3Q�.+�� $	��100��$	��100��.+�A� -	    A�Name$ OuterSpinner��$	��100��$	��100��$��4������ .���  � Color$ 	#FFFF8C42�� � Offset$ 0Q�� $ 	#40FF8C42��$ 1Q�z��� -
   SpinnerRotation�� �� CenterX$ 50Q� �� CenterY$ 50Q�A� -   $ MiddleSpinner��$��80��$��80��$��3������ .��� $ 	#FFFFA366��$ 0Q�� $ 	#40FFA366��$ 1Q�z��� -   MiddleSpinnerRotation��$ 40Q�$ 40Q� �� Angle$ 45Q�A� -   $ InnerCircle��$��60��$��60����
� .��� $ 	#40FFB380��$ 0Q�� $ Transparent��$ 1Q�z��� -   PulseTransform�� �� CenterX$ 30Q� �� CenterY$ 30Q�    ShowLoadingAnimation �� 
TargetName �� TargetProperty W� From	 W� To g� Duration  W� EasingFunction/  )System.Windows.Media.Animation.CircleEase7  1System.Windows.Media.Animation.EasingFunctionBase!   
EasingMode0  *System.Windows.Media.Animation.ElasticEase"   Oscillations#   Springiness$ g� RepeatBehavior% g� AutoReverse-	  'System.Windows.Media.Animation.SineEase  HideLoadingAnimation/
  )System.Windows.Media.Animation.BounceEase��%�  &	       &	 �    �� .��W� $ LoadingOverlay��$ Opacity�$ 0X�$ 1X�$ 0:0:0.3D�    $! EaseOut=�W� $ LoadingScale��$ ScaleX�$	 0.3X�$ 1X�$ 0:0:0.4D�    $! EaseOut=�$" 2��$# 3Q�W� $ LoadingScale��$ ScaleY�$	 0.3X�$ 1X�$ 0:0:0.4D�    $! EaseOut=�$" 2��$# 3Q�W� $ SpinnerRotation��$ Angle�$ 0X�$	 360X�$ 0:0:2D�$$ Forever��W� $ MiddleSpinnerRotation��$ Angle�$	 360X�$ 0X�$ 0:0:1.5D�$$ Forever��W� $ PulseTransform��$ ScaleX�$	 0.8X�$	 1.2X�$ 0:0:1D�% . $$ Forever��  	  $! 	EaseInOut=�W� $ PulseTransform��$ ScaleY�$	 0.8X�$	 1.2X�$ 0:0:1D�% . $$ Forever��  	  $! 	EaseInOut=��� .��W� $ LoadingOverlay��$ Opacity�$ 1X�$ 0X�$ 0:0:0.2D�    $! EaseIn=�W� $ LoadingScale��$ ScaleX�$	 0.3X�$ 0:0:0.3D�  
  $! EaseIn=�W� $ LoadingScale��$ ScaleY�$	 0.3X�$ 0:0:0.3D�  
  $! EaseIn=�