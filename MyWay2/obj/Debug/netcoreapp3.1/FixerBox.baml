   M S B A M L   `   `   `  ����C  ?Paneless, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PanelessPaneless  5          Paneless.FixerBox   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      (localclr-namespace:Paneless     5        � 	Resources  5
         
LinkButton �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value��%�  &	        �� 5      " ��6   .�5      ��  � �� 5      �� 5      " ��6   .R�5      �� $
��None{�6   �� 5      5   	   5      5      �� 5       � ��$	 3,1q�6   �� 5       � ��$	 0,0q�6   �� 5       �@�Cursor$
 Hand��6    5      P� 5       �@J�IsMouseOver$
 true��6%   . �5      ��  � �� �	#FF2768B66$   5      5      5      .��5      ��  �� CornerRadius$ 12��6
   $	��400��6   $��8,8,8,8q�6(   ��5       �� 5!      	 �� EndPoint$	 0.5,1&�6   
 �� 
StartPoint$
 0.5,0&�6*   .��5"      �  � Color$ 	#FFC5C0C0��6    � Offset$ 1Q�6%   � 5#      $ White��6   $ 0Q�6!   5$      ._�5&      �  � RowDefinitions 5'      �� 5(      $
��auto �6   �� 5)      $
��auto �6   �� 5*      .+�5-      � $��0��6
    � ColumnDefinitions 5.      �� 5/      $	��120 �6   �� 50       52      �� 53      $
��auto �6   $��45��6#   �� 54      $
��auto �6   $��55��6#   .+�56      �� $��1q�6   ���	#FFB4B4B4$��0��69   $��0��6F   $��2��6V   $	��100��6g   $	��100��6v   $��bottom=�6�   ._�57      �� -   5        $��FixerImg��57      $��/windows.jpg��6   58      �� 59      -   5        $��
FixerTitle��5:      $��0��59      $��1��6   $��15(�5:      $��Arial Rounded MT Bold)�6&   $
��Bold"�6I   $
��Wrap=�6[   $��right=�5;      $��Bottom=�6"   $��0,0,10,4q�5<      '%Title for some annoying Windows Thing6?   �� 5=      -   5        $��FixedButton��5=      $ 12,0,0,0��6    $��0q�68   $��1��6L   $��1��6Y   ���	#FFC5C5C5 � Cursor$
 Hand��6�   $y�Hidden=�6�   ._�5>      � .+�5?      �� -   5        ���	#FFDAFFCC$	��100��5?   #   $��40��6/    �� RadiusX$ 20��6;    �� RadiusY$ 20��6H   $��Center=�6U   ���	#FF71CF4C$��1��6�   6�   A� 5@      -   5        ���	#FF68CF4C$��30��5@   !   $��30��6-   $��Center=�68   $��0,0,55,0q�6u   �� 5A      -   5        $��Center=�5A      $��Center=�6+   $��Arial Rounded MT Bold)�6F   $��14(�6i   $y�Visible=�6w   $��30,0,0,0q�6�   .��6�   FIXED!6�   5B      5C      �� 5D      -   5        $��	FixButton��5D      $ 12,0,0,0��6   $��0q�66   $��1��6J   $��1��6W   ���	#FFC5C5C5$
 Hand��6~   $y�Visible=�6�   ._�5E      � .+�5F      �� -   5        ���	#FFFFF2CC$	��100��5F   #   $��40��6/   $ 20��6;   $ 20��6H   $��Center=�6U   ���	#FFCFB24C$��1��6�   6�   A� 5G      -	   5        ���	#FFCFB24C$��30��5G   "   $��30��6.   $��Center=�69   $��55,0,0,0q�6v   �� 5H      -
   5        $��40��5H      $
��Wrap=�6   $��Center=�61   $��Center=�6N   $��	-30,0,0,0q�6i   $��Arial Rounded MT Bold)�6|   $��14(�6�   $y�Visible=�6�   Click to fix6�   5I      5J      5K      � 5L      -   5         �Name$ 	FixerTags��5L   F   $��1��6   $
��auto��6   $��right=�6*   .+�5M      �� #��[  6   ���	#FF7A7A7A$��#Tags��6H   5N      �� 5O      -   5         ��Name$ 	FixerDesc��5O      $��2��6"   $��1��6/   $
��Wrap=�6?    �� Padding$ 	10,6,10,6q�6S   $��15(�6g   �� 6w   $���This is test text. It has to be a bit long to get a sense for how this would look in practice. Not sure how long, but at least as lengthy as my normal rants about these things. It's anyone's guess what that actually means though... I kind of make it up as I go. What happens if I make it even bigger though? Cool. It expands :)��6{   �� 6�  �� 6�  6�  5P      5Q      5R      