   M S B A M L   `   `   `  ����C  ?Paneless, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PanelessPaneless  5          Paneless.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      (localclr-namespace:Paneless     5        � Title$  PaneLess��5      $	��850��6   $
��1420��6!   ���	#FF323030 � 	Resources 5	         
LinkButton �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value��%�  &	        �� 5
      " ��6   .�5      ��  � �� 5      �� 5      " ��6   .R�5      �� $
��None{�6   �� 5      5   	   5      5      �� 5       � ��$	 3,1q�6   �� 5       � ��$	 0,0q�6   �� 5       �@�Cursor$
 Hand��6    5      P� 5       �@J�IsMouseOver$
 true��6%   . �5      ��  � �� �	#FF2768B66$   5      5      5      .��5      � 	 � RowDefinitions	 5      �� 5       $��80 �6   �� 5!      $
��auto �6   $��25��6!   �� 5"      $��60 �6   �� 5#      .+�5&      � -   5        $��LogoArea��5&   	   $��0��6   
 � ColumnDefinitions
 5'      �� 5(      .+�5+      � $��0��6   $��Center=�6   $��center=�6<   .+�5,      �� $��Graphics/logo.png��6   $��5,5,5,5q�6'   �� 5-      $��PaneLess��6   $��Microsoft Sans Serif)�6   $��56(�6A   ���	#FF4472C4$
��Bold"�6f   5.      5/      A� 51      -   5        $��
StatusArea��51      $��1��6    A� Columns$ 1��6/   ��52      �� 53       �� Color$ 	#FFF0F0F0��6   .+�55      �� -   5        $��TempWillBeRemoved��55      6&   56      � 58      -   5        $��FiltersArea��58   	   $��2��6   
 59      �� 5:      $	��120 �6   �� 5;      �� 5<      $	��120 �6   .+�5?      �� $��0��6    �� CornerRadius$ 5,5,5,5��6   $��1q�63    � Cursor$
 Hand��6G   ���	#FF828282 �� Padding$ 5,5,5,5q�6m   $��3q�6   ._�5@      �� -   5        $��Filter��5@   <   $��Stretch=�6   $��0��6,   $��25��6J   ���	#FF989898
 � Tag$ placeholder��6�   .��6�    5A      ��%�   '��       �� 5B      " ��6   .�5C   	   ��  �@��CornerRadius$ 3��6(   �� 5D   	    �@�ToolTip$J DType any text here to filter fixes to only those with matching text.��6#   5E   	   5F      .��6   5G      5H      �� 5I      $��1��6   $ 5,5,5,5��6   $��1q�63   $
 Hand��6G   ���	#FF828282$ 5,5,5,5q�6m   $��3q�6   ._�5J      � 	 5K      �� 5L      $��25 �6   �� 5M      .+�5P      �� -   5        $��TagGuideNone��5P      $��0��6   $��Stretch=�6.   
�������$��Center=�6c   .��6�   ��Click tags on fixes (ex. #Windows Explorer) to see only fixes in that category (you can click multiple in succession to further refine the listing)6  �� 5Q      -   5        $��TagGuideSome��5Q      $��0��6   $��Stretch=�6.   
�������$��Center=�6c   $y�	Collapsed=�6�   .��6�   :8Click any tag in this area to remove it from the filter.6�   � 5R      -   5        $��
ActiveTags��5R      $��1��6   $��Center=�60   $
��auto��6M   5T      5U      5V      �� 5W      $��2��6   $ 5,5,5,5��6   $��0q�63   ���	#FF828282._�5X      �� -	   5        $��ClearButton��5X   F   $��0��6   $��3q�6   $��0��6%   $��2��65   ���	#FFF5EED9$��Stretch=�6p   ���	#FFE0C567$��1q�6�   $��Stretch=�6�   .��5Y      �� -
   5        $��ClearButtonTxt��5Y      $��Clear Filter and Tags��6'   $
��Wrap=�6D    �� TextAlignment$ Center=�6X   $
��auto��6o    5Z      ��%�   '��       �� 5[      " ��6   .�5\   	   ��  �@��CornerRadius$ 4��6(   �� 5]   	    �@�ToolTip$N HRemoves any text filters or selected tags and shows all available fixes.��6#   5^   	   5_      5`      5b      5c      � 5e      $��3��6	   
 5f      �� 5g      �� 5h      $	��120 �6   .+�5k      �� $��0��6   $��Visible=�6"   .��5l      � -   5        $��
FixersArea��5l       � Orientation$ 
Horizontal=�6"   $
��auto��6;   6K   5m      � 5o      $��1��6   $��3q�6   .+�5p      A� $ 1��6   .+�5r      �� $��Fixes��6   ���	#FFD7ECFF$
��Bold"�64   $��Stretch=�6F   $��Center=�6d   ���	#FFD7ECFF$��0,0,0,2q�6�   �� 5s      -   5        $��FixAllButton��5s      $��3q�6"   $��Apply visible fixes��6-   ���	#FFD2FFB2���	#FF52C961$��1q�6z   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5t      ��%�   '��       �� 5u   	   " ��6   .�5v   
   ��  �@��CornerRadius$ 4��6)   �� 5w   
    �@�ToolTip$N HApplies visible fixes. Fixes hidden by filters and tags are not applied.��6$   5x   
   5y   	   5z      �� 5|      $��
Save prefs��6   ���	#FFD7ECFF$
��Bold"�69   $��Stretch=�6K   $��Center=�6i   ���	#FFD7ECFF$��0,0,0,2q�6�   �� 5}      -   5        $��
SaveButton��5}      $��3q�6!   $��Save to Prefs.txt��6,   ���	#FFD2FFB2$��Stretch=�6_   ���	#FF52C961$��1q�6�   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5~      ��%  '��       �� 5   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$� �Save all button positions to the prefs file. Note that each time you toggle a fix, that fix is recorded individually in the prefs files as well.��6$   5�   
   5�   	   5�      �� 5�      -   5        $��BackupButton��5�      $��3q�6#   $��Backup Prefs.txt��6.   ���	#FFD2FFB2$��Stretch=�6`   ���	#FF52C961$��1q�6�   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$M GCreates a backup file of the current Prefs.txt with a date/time prefix.��6$   5�   
   5�   	   5�      �� 5�      $��
Load prefs��6   ���	#FFD7ECFF$
��Bold"�69   $��Stretch=�6K   $��Center=�6i   ���	#FFD7ECFF$��0,0,0,2q�6�   �� 5�      -   5        $��LoadPrefButton��5�      $��3q�6$   $��Load Prefs File��6/   ���	#FFF5EED9���	#FFE0C567$��1q�6y    5�      ��%  '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$� �Select a preferences file to compare to your current settings. Does not make changes until/unless you click the Match Prefs file button!��6$   5�   
   5�   	   5�      �� 5�      -   5        $��MatchAllButton��5�      $��3q�6$   $��Match Prefs File��6/   ���	#FFF5EED9���	#FFE0C567$��1q�6y    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$B <Toggle fixes on and off to match the current preference file��6$   5�   
   5�   	   5�      �� 5�      -   5        $��CancelPrefButton��5�      $��3q�6&   $��Cancel Load��61   ���	#FFCACACA���	#FFE0C567$��1q�6w   $y�	Collapsed=�6�    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$R LClick to cancel the loaded preferences and return to your previous settings.��6$   5�   
   5�   	   5�      �� 5�      $��Snark Level��6   ���	#FFD7ECFF$
��Bold"�6:   $��Stretch=�6L   $��Center=�6j   ���	#FFD7ECFF$��0,0,0,2q�6�   �� 5�      -   5        $��FullSnarkButton��5�      $��3q�6&   $��
Full Snark��61   ���	#FFDADADA$��Stretch=�6]   ���	#FFB4B4B4$��1q�6�   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$e _Show the fix descriptions with the full and un-edited grump they were originally designed with.��6$   5�   
   5�   	   5�      �� 5�      -   5        $��NoSnarkButton��5�      $��3q�6$   $��No Snark��6/   ���	#FFDADADA$��Stretch=�6Y   ���	#FFB4B4B4$��1q�6�   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$5 /Minimal and factual descriptions without snark.��6$   5�   
   5�   	   5�      �� 5�      -   5        $��
DiagButton��5�      $y�	Collapsed=�6!   $��3q�68   $
��Diag��6C   ���	#FFDADADA$��Stretch=�6i   ���	#FFB4B4B4$��1q�6�   $��Normal"�6�   $��Arial Rounded MT Bold)�6�    5�      ��%�   '��       �� 5�   	   " ��6   .�5�   
   ��  �@��CornerRadius$ 4��6)   �� 5�   
    �@�ToolTip$T NButton trigger to test things for diagnostic purposes. You shouldn't see this.��6$   5�   
   5�   	   5�      5�      � 5�      -   5        $��SpaceFiller��5�      $��1��6#   5�      5�      5�      5�      5�      