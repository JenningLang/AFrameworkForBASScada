   M S B A M L   `   `   `  ����H  DMahApps.Metro, Version=1.6.0.0, Culture=neutral, PublicKeyToken=nullV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5   	    FrontFramework4   .FrontFramework.Language.Views.DictionaryEditor   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5   	     � Icon$  ../../resources/icon.png��5   	   $	��500��6)   $	��310��66    � 
ResizeMode$ NoResize=�6B   $��16(�5   	   $��等线)�5   	   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   6)   �� 5      $
��Auto �6   6,   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5       $��0��6      AutoGenerateColumns .  $��5q�6f   $��Stretch=�5      $��Stretch=�61   
����ح�$��1q�6e      RowHeaderWidth$ 0Q�5         CanUserAddRows .  ��5   2   ��    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 1* 6%   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 60   �� *	chinese+  5      $ 1* 6%   	 60   �� *	english+5      � 5   
   $��1��6   
 � ColumnDefinitions
 5      �� 5      6%   �� 5      6%   �� 5      6%   .+�5      �� -   5        $��	buttonAdd��5      $��0��6&   $��Stretch=�66   $��Center=�6T   $��16(�6�   $��等线)�6�   ���	#FF337AB7
�������$��5q�5   ;   .��6F   Button5       �� 5!      -   5        $��
buttonEdit��5!      $��1��6'   $��Stretch=�67   $��Center=�6U   $��16(�6�   $��等线)�6�   ���	#FF5FBF5F
�������$��5q�5"   ;   .��6F   Button5$      �� 5%      -   5        $��	buttonDel��5%      $��2��6&   $��Stretch=�66   $��Center=�6T   $��16(�6�   $��等线)�6�   ���	#FFD9534F
�������$��5q�5&   ;   .��6F   Button5(      5)      5*      5+      