   M S B A M L   `   `   `  ���� g  cGenesyslab.Desktop.Modules.YoutubeWorkItem, Version=8.5.122.8, Culture=neutral, PublicKeyToken=nullyAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls3Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls  ` \Genesyslab.Desktop.Modules.Sdr.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common.Genesyslab.Desktop.Modules.Sdr.Common.Controls R NTomers.WPF.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Tomers.WPF.Localization;assembly=Tomers.WPF.LocalizationTomers.WPF.Localization A    ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     [DeleteControlAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls     ~commonkclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common     _localizationFclr-namespace:Tomers.WPF.Localization;assembly=Tomers.WPF.Localization       � 	Resources   �� MergedDictionaries �� Source��  �� $r lpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Resources/YoutubeMainStyles.xaml?�%    .��� -    �Name$ CommentChatBox��$��1q�$
��Auto������  �� Path$ Width� �� RelativeSource ��  �� AncestorType" >� �� Mode$ FindAncestor=� � ColumnDefinitions �� $��24 ��� $��* �.+�@  :Genesyslab.Desktop.Modules.Sdr.Common.Controls.AuthorImage  $��0��$	��Top=�$��24��$��24��	 � DataContext	 �� *AuthorImage+�� $��1��
 �� CornerRadius$
 2��$	��5 0q�$	��250��$��Stretch=�$��1q�
�������$
��Auto������  �� Color$ Transparent�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     Color$
 Gray��   ShadowDepth$ 1Q�   Opacity$	 0.5Q�._��� $��5q�.+��� $��Center=�$
��Wrap=�$
��Auto����  �� BaselineAlignment$	 Top=�.F��� $	��Top=�$
��Wrap=�$
��Auto��$
��Bold"�
���   ����� *
AuthorName+�� $	��200��$��2 0 -1 0q�$��0q�.���� -    ��Name$ CommentTextBox��$	��Top=� �� TextWrapping$
 Wrap=�$
��Auto�����#313335
������ 
������ $��0q�$��0q�$��0q� �� 	MaxLength$ 10000�� �� 
IsReadOnly �� *IsReplyTextReadOnly+ � Cursor �� *ReplyCurser+���� *Text+�� $	��0 5q�
������� J� Opacity$	 0.5Q�� $��0 0 0 0q� �� $��* ��� $��2* �.+��� $��0��$��0q����#86888a$��Center=�$
��Left=�$��10(����� *
DateText+�� $��1�� �� Orientation$ 
Horizontal=�$��Center=�$��Right=�.+�D   >Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.DeleteInfo  -     Name$ CommentDeleteInfo��y��� *DeleteDetailsVisibilty+?  9Genesyslab.Desktop.Modules.Sdr.Common.Controls.IconButton  -     Name$ LikeBtn��   YoutubeLikeButton#��[  $y�Hidden=�'  !Tomers.WPF.Localization.Translate
   Uid$ Youtube.Actions.Like��{��� *	CanLike+   
ButtonText      Default$
 Like�� � ToolTip   $
 Like��  -   $ ReplyBtn��
�������  CommentButton#��[ $ Youtube.Actions.Reply��y��� *ReplyButtonVisibilty+   $ Reply��   $ Reply��  -   $ 	DeleteBtn��
�������  DeleteButton#��[ $ Youtube.Actions.Delete��y��� *DeleteButtonVisibilty+   $ Delete��   $ Delete��@  :Genesyslab.Desktop.Modules.Sdr.Common.Controls.EmojiPicker  -   $��4q�   Fill
 �����$��Right=�y���  �� ElementName$ SendBtn��$ 
Visibility�  -   $ SendBtn��
�������  
SendButton#��[ $ Youtube.Actions.Send��y��� *SendButtonVisibility+   $
 Send��   $
 Send��