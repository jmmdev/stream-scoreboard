   M S B A M L   `   `   `  ���� E  AScoreboard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Scoreboard
Scoreboard      Scoreboard.Message   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:Scoreboard       � 
ResizeMode$  NoResize=� � Icon$ icon.ico��   
Background#���   
���   �$��1q� � WindowStyle$
 None=� � WindowStartupLocation$ CenterOwner=� � Title$  ��$	��140��$	��300�� � 	Resources   StandardButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� CornerRadius  StandardButtonIdle �� Triggers P� Property P� Value �� 
TargetName  StandardButtonMouseOver  StandardButtonPressed �� Triggers��%   &	       �� " ��.���  �@�Cursor$
 Hand����  � �� �#ffffff��  � ��$ 14(���  � �� �� "	 ��.R��� $��Border��$
 2��$��0q�#���  #���  ._��� $��2q�$��Center=�$��Center=���.  P�  �@J�IsMouseOver . . ��� $ Border��# �   � ��P�  � �� . . ��� $ Border��# �   � �� P�  � {� .  . ���  �@J�Opacity$	 0.5Q���  �@�Cursor$ Arrow��P�  �@J�IsMouseOver . . ���  � �� �#eeeeee.��� .+�� -   $��textGrid��$��80��$��16 12q�$	��Top=�.+��� -   $��lbText��$
��Wrap=�
�������$��14(�{�.   �� TextAlignment$ Center=�$��Center=��� -   $��btnOk��#���  $	��108��$��28��$��Center=�$	��Top=�$��	0 100 0 0q�#���    ButtonIdleText#���  $
��Bold"��� -   $��btnYes��#���  $	��108��$��28��$
��Left=�$	��Top=�$��
30 100 0 0q�#���  #���  $y�Hidden=�$
��Bold"��� -   $��btnNo��#���  $	��108��$��28��$��Right=�$	��Top=�$��
0 100 30 0q�#���  #���  $y�Hidden=�$
��Bold"�