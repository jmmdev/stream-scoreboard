   M S B A M L   `   `   `  ����E  AScoreboard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Scoreboard
Scoreboard  5   	       Scoreboard.Message   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:Scoreboard     5   	     � 
ResizeMode$  NoResize=�5   	    � Icon$ icon.ico��5	   	      
Background#���   5
   	   
���   �$��1q�5   	    � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterOwner=�5   	    � Title$  ��5   	   $	��140��6   $	��300��6    � 	Resources 5        StandardButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� CornerRadius  StandardButtonIdle �� Triggers P� Property P� Value �� 
TargetName  StandardButtonMouseOver  StandardButtonPressed �� Triggers��%u  &	       �� 5   
   " ��6'   .�5      ��  �@�Cursor$
 Hand��6'   �� 5       � �� �#ffffff6+   �� 5       � ��$ 14(�6)   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $��Border��6!   $
 2��6/   $��0q�6@   #���  6T   #���  5      ._�5      �� $��2q�6/   $��Center=�5   "   $��Center=�5   "   ��. 5   "   5       5       P� 5!       �@J�IsMouseOver . . �5"   "   �� $ Border��6)   # �  6S    � ��5#      P� 5$       � �� . . �5%   "   �� $ Border��6)   # �  6S    � ��5&      5(      5*       5,      P� 5-       � {� .  . �5.      ��  �@J�Opacity$	 0.5Q�60   �� 5/       �@�Cursor$ Arrow��6/   50      P� 51       �@J�IsMouseOver . . �52      ��  � �� �#eeeeee63   53      55      56      .��57      � .+�58   
   � -   5        $��textGrid��58      $��80��6   $��16 12q�6+   $	��Top=�6:   .+�59      �� -   5        $��lbText��59      $
��Wrap=�6&   
�������$��14(�6M   {�.   �� TextAlignment$ Center=�6m   $��Center=�6�   5:      �� 5;   
   -   5        $��btnOk��5;      #���  6   $	��108��6G   $��28��6S   $��Center=�6_   $	��Top=�6|   $��	0 100 0 0q�6�   #���  6�     ButtonIdleText#���  6�   $
��Bold"�6  �� 5<   
   -   5        $��btnYes��5<      #���  6   $	��108��6H   $��28��6T   $
��Left=�6`   $	��Top=�6{   $��
30 100 0 0q�6�   #���  6�   #���  6�   $y�Hidden=�6  $
��Bold"�60  �� 5=   
   -   5        $��btnNo��5=      #���  6   $	��108��6G   $��28��6S   $��Right=�6_   $	��Top=�6{   $��
0 100 30 0q�6�   #���  6�   #���  6�   $y�Hidden=�6  $
��Bold"�6/  5>      5?      