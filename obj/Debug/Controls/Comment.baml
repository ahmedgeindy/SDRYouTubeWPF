   M S B A M L   `   `   `  ����g  cGenesyslab.Desktop.Modules.YoutubeWorkItem, Version=8.5.122.8, Culture=neutral, PublicKeyToken=nullyAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls3Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls  5      ` \Genesyslab.Desktop.Modules.Sdr.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common.Genesyslab.Desktop.Modules.Sdr.Common.Controls 5      R NTomers.WPF.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Tomers.WPF.Localization;assembly=Tomers.WPF.LocalizationTomers.WPF.Localization 5      A    ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      [DeleteControlAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls     5      ~commonkclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common     5      _localizationFclr-namespace:Tomers.WPF.Localization;assembly=Tomers.WPF.Localization     5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $r lpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Resources/YoutubeMainStyles.xaml?�6%   %    5      .��5      � -   5         �Name$ CommentChatBox��5      $��1q�6#   $
��Auto��6.   ��5      ��  �� Path$ Width� �� RelativeSource ��  �� AncestorType" >� �� Mode$ FindAncestor=� � ColumnDefinitions 5   
   �� 5      $��24 �6   �� 5      $��* �6   .+�5   
   @  :Genesyslab.Desktop.Modules.Sdr.Common.Controls.AuthorImage  $��0��6   $	��Top=�6-   $��24��6E   $��24��6P   	 � DataContext	 6\   �� *AuthorImage+�� 5   
   $��1��6   
 �� CornerRadius$
 2��6!   $	��5 0q�62   $	��250��6?   $��Stretch=�5      $��1q�5      
�������$
��Auto��5      ��5      �� 5       �� Color$ Transparent��6"    J� Effect 5!      3  -System.Windows.Media.Effects.DropShadowEffect  5"         Color$
 Gray��6#      ShadowDepth$ 1Q�60      Opacity$	 0.5Q�6A   ._�5%      �� $��5q�6   .+�5&      �� $��Center=�6   $
��Wrap=�67   $
��Auto��6K   �� 5'   &    �� BaselineAlignment$	 Top=�68   .F�5(   *   �� $	��Top=�64   $
��Wrap=�5)   0   $
��Auto��5*   0   $
��Bold"�5+   0   
���   ���5-   0   �� *
AuthorName+5.   '   5/   #   �� 51      $	��200��6   $��2 0 -1 0q�6/   $��0q�6A   .��53      �� -   5         ��Name$ CommentTextBox��54      $	��Top=�53       �� TextWrapping$
 Wrap=�66   $
��Auto��55      ���#313335
������ 
������ $��0q�57      $��0q�6.   $��0q�69    �� 	MaxLength$ 10000��5:       �� 
IsReadOnly 58      �� *IsReplyTextReadOnly+ � Cursor 59      �� *ReplyCurser+��5;      �� *Text+5=      �� 5?      $	��0 5q�6   
������� J� Opacity$	 0.5Q�6;   � 5A      $��0 0 0 0q�6    5B      �� 5C      $��* �6+   �� 5D      $��2* �6+   .+�5G      �� $��0��6    $��0q�5H   $   ���#86888a$��Center=�5J   $   $
��Left=�5K   $   $��10(�5L   $   ��5G   0   �� *
DateText+5L   $   �� 5N      $��1��6!    �� Orientation$ 
Horizontal=�61   $��Center=�6J   $��Right=�6e   .+�5P      D   >Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.DeleteInfo  -   5          Name$ CommentDeleteInfo��5P   3   y�6N   �� *DeleteDetailsVisibilty+?  9Genesyslab.Desktop.Modules.Sdr.Common.Controls.IconButton  5R      -   5          Name$ LikeBtn��5R   ,      YoutubeLikeButton#��[  6=   '  !Tomers.WPF.Localization.Translate
   Uid$ Youtube.Actions.Like��5U   ,   {�5S   ,   �� *	CanLike+y�5T   ,   �� *LikeButtonVisibilty+   
ButtonText 5V   ,        Default$
 Like�� � ToolTip 5W   ,     $
 Like��5X   ,     5Z      -   5        $ ReplyBtn��5Z   ,   
�������  CommentButton#��[ 5[   ,   $ Youtube.Actions.Reply��5]   1   y�5\   1   �� *ReplyButtonVisibilty+ 5^   1     $ Reply�� 5_   1     $ Reply��5`   1     5b      -   5        $ 	DeleteBtn��5b   ,   
�������  DeleteButton#��[ 5c   ,   $ Youtube.Actions.Delete��5e   0   y�5d   0   �� *DeleteButtonVisibilty+ 5f   0     $ Delete�� 5g   0     $ Delete��5h   0   @  :Genesyslab.Desktop.Modules.Sdr.Common.Controls.EmojiPicker  5j      -   5        $��4q�5j   -      Fill
 �����$��Right=�5k   _   y�5j   8   ��  �� ElementName$ SendBtn��$ 
Visibility�5k   _     5m      -   5        $ SendBtn��5m   ,   
�������  
SendButton#��[ 5n   ,   $ Youtube.Actions.Send��5p   .   y�5o   .   �� *SendButtonVisibility+ 5q   .     $
 Send�� 5r   .     $
 Send��5s   .   5v      5y      5{      5|      5~      5      