   M S B A M L   `   `   `  ����J  FVk_Client_by_MK, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:VKMessenger_by_MKVKMessenger_by_MK  5   	   "    VKMessenger_by_MK.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:VKMessenger_by_MK     5   	     � Title$  VKMessenger by MK��5   	   $	��700��6#   $	��900��60   ���#597da3 � 
ResizeMode$ CanMinimize=�6Q    � 	Resources 5	         slideLeftToRight �� TargetProperty g� AccelerationRatio g� DecelerationRatio �� 
TargetName g� Duration W� From		 W� To��%�   &	        �� 5
   
   $, &RenderTransform.(TranslateTransform.X)�5      $ .4Q�5      $ .4Q�5      .��5      W� $ Pages��6   $ 0:0:0.6D�6<   $	 300X�6O   $	 0X�6Z   5      5      .��5      � 
 � RowDefinitions
 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      �� 5       � ColumnDefinitions 5!   
   �� 5"      �� 5#      �� 5$      �� 5%      �� 5&      �� 5'       5)   
    �� 
TargetType �� Property �� Value��%|   '��       �� 5*      " ��6   .�5+      ��  � ��
 �����6/   �� 5,       � ��$ Italic$�6.   5-      5.      .+�5/   
   �� ���#597da3$��6��6*    �� Orientation$ 
Horizontal=�6>   $��
10,5,10,51q�6W   $��2��6k    � IsSharedSizeScope . .+�50      �  51      �� 52       z� SharedSizeGroup$
 Menu��6'   .+�54      �� $��Groups��6   $��30(�6*   ���	#00FFFFFF$	��0.1q�6P   $��Italic$�6f   
�������6y   55      � 56       57      �� 58      $
 Menu��6'   .+�5:      �� -   5        $��Messages��5:      $��30(�6,   ���	#00FFFFFF$	��0.1q�6o   $��Italic$�6�   
�������6�   5;      � 5<       5=      �� 5>      $
 Menu��6'   .+�5@      �� $��Options��6   $	��300��6+   $��30(�67   ���	#00FFFFFF$	��0.1q�6]   $��Italic$�6s   
�������6�   5A      5B      �� 5C   
   -   5        $��Welcome��5C      $��Welcome!��6   $��1��62   $��4��6B   $��4��6O   $��3��6c   $	��100(�6t   $��	97,0,42,0q�6�    � 5D   
   -   5        $��Pages��5D      $��0��6   $��1��6-   $��6��6:   $��12��6N     � NavigationUIVisibility$ Hidden=�6`   6�   5E      5F      