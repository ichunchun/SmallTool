   M S B A M L   `   `   `  ����D  @SmallTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:SmallTool	SmallTool  5   	       SmallTool.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:SmallTool     5   	     � Title$  Yang's Tool��5   	   $	��410��6   $	��340��6+    � WindowStartupLocation$ CenterScreen=�67   
������  � WindowStyle$
 None=�6u    � AllowsTransparency .  � 	Resources 5	         TextboxStyle �� 
TargetType �� Property �� Value��%)  &	        �� 5
   
   " ��6%   .�5      ��  � �� �#1f1f1f6=   �� 5       � ��
 �����6;   �� 5       � ��$ 0q�60   6<   �� 5       � ��$ Center=�6;   6L   �� 5       � ��$ Center=�69   6J   �� 5       � ��$ 30(�6)   66   5      5      .��5      � .+�5   
   �� -   5        ���#1f1f1f �� CornerRadius$ 12��5      5      �� 5   
   	 �� Orientation$	 
Horizontal=�6   $��Right=�6.   $��	0,16,16,0q�6J   .+�5      �� -   5        $��20��5      $��20��6!   $	��Top=�5        closebtn#��[ 5      
���  ��5      5      �� 5    
   $	 Vertical=�6   $��
20,60,20,0q�6,   .+�5!      �� -   5        
 ��Name$
 PCName��5!   K   #��[  6   $��AAA888��6=    � ToolTip$T N计算机名称会在重启后才会显示修改，已我的电脑里的为准��6[   �� 5"      -   5        $
 IP��5"   `   #��[  6   $��192.168.123.21��6<   $
��0,20q�6R   6`   �� 5#      -   5        $
 NetCard��5#   G   #��[  6   $��	以太网��6<   6G   �� 5$      -   5         ��Name$ checkbox��5$   �   $��选中后变为SaveTOJson��6   $��Center=�61   $
��0,30q�6O   
�������6�   �� 5%      -   5         ��Name$	 btn��5%   W   $��
ChangeInfo��6   $	��220��6*   $��60��66    
 Runbtn#��[ 6d   �� 5&      $	 
Horizontal=�6   $��0,10,0,0q�62   $��Center=�6D   .+�5'      �� -   5        $	����5'        	optionbtn#��[ 6,   
�������6o   �� 5(      -	   5        $��20,0,0,0q�5(      $	����6+   #��[ 6>   
�������6{   �� 5)      -
   5        $ autochangebtn��5)   d   $��20,0,0,0q�6   $	����6+   #��[ 6>   
�������$y�	Collapsed=�6�   6�   5*      5+      5,      5-      