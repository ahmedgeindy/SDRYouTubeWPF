   M S B A M L   `   `   `  ����`  \Genesyslab.Desktop.Modules.Sdr.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�kclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common.Genesyslab.Desktop.Modules.Sdr.Common.Controls  5      g cGenesyslab.Desktop.Modules.YoutubeWorkItem, Version=8.5.122.8, Culture=neutral, PublicKeyToken=nullyAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls3Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls 5      E   ?Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.YoutubeView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ~commonkclr-namespace:Genesyslab.Desktop.Modules.Sdr.Common.Controls;assembly=Genesyslab.Desktop.Modules.Sdr.Common     5      VControlsAclr-namespace:Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls     5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5	      $r lpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Resources/YoutubeMainStyles.xaml?�6%   %    5      .��5      � -   5         �Name$ myGrid��5      $��1q�6   $��Stretch=�6'   
������� � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   ;   5Genesyslab.Desktop.Modules.Sdr.Common.Controls.Header  $��0��6   $��0��6%   $��2��65   $��1q�6I   
�������   HeaderImageSource$h bpack://application:,,,/Genesyslab.Desktop.Modules.YoutubeWorkItem;component/Images/YoutubeLogo.png��5      >  8Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Post  5   
   -   5          Name$ PostControl��5   A   $��0��6   $��1��6(   $	��550��65   $
��Left=�5       � DataContext 5   V   �� *Post+5      A  ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment  5    
   -   5          Name$ 	InComment��5    8   A  ;Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.Comment	   Loaded$	 InComment_Loaded��5!   ^   $��2��5       $��0��6(   $	��550��6K   $
��Left=�6W   $	��6 0q�6r   
   SendButtonVisibility$
 	Collapsed=�5!       6<   �� *	InComment+6^   �� 5$   
   -   5        $��
icTodoList��5$      $��3��6)   $��0��66   $��
29 10 10 0q�6F   $
��Left=�6Z   $
��Auto��5%      
������ ��6   �� *ReplyMessages+��5&      �� 5'      .R�5(           	SendClick OutComment_SendClick5)   '      DeleteClick InComment_DeleteClick5*   '   $	��521��5(   '   $
��Left=�63   $	��6 0q�6N      ReplyButtonVisibility$ 	Collapsed=�5-   '      LikeButtonVisibility$ 	Collapsed=�5.   '      DeleteButtonVisibility 5+   '   �� *Comment.DeleteButtonVisibilty+
 5,   '   �� *SendButtonVisibility+5.   '   5/      51      �� 53   
   -   5        $��OutCommentsList��53      $��4��6.   $��0��6;   $��	0 10 10 0q�6K   $
��Left=�6^   $
��Auto��54      
������ ��6   �� *OutComments+��55      �� 56      .R�57         OutComment_SendClick58   '    InComment_DeleteClick5;   '   $	��550��57   '   $
��Left=�63   $	��6 0q�6N   $ 	Collapsed=�59   '   $ 	Collapsed=�5:   '   
 5<   '   �� *SendButtonVisibility+ 5=   '   �� *DeleteButtonVisibilty+5>      5@      D  >Genesyslab.Desktop.Modules.YoutubeWorkItem.Controls.AddComment  5B   
   -   5          Name$ AddCommentControl��5B      $��5��69   $��Center=�5C      $
��Left=�69   $y�	Collapsed=�5D       65   �� *
AddComment+5F      5G      