   M S B A M L   `   `   `  ����  Hearthstone"    Hearthstone.SimulationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Hearthstone Simulator��5   	   $
��1000��5   	   $	��900��5   	   $
��1300��5   	   $
��1200��5   	   $��20(�5	   	   .��5
      ]� .+�5   
   � $	��Top=�6   $��Stretch=�6$   $��Stretch=�6@   $	��500��6^   $��10q�6k    � ColumnDefinitions 5      �� 5      $	��160 �6#   �� 5       � RowDefinitions 5      �� 5      $
��0.6* �6    �� 5      �� 5      �� 5      �� 5      �� 5      $
��0.6* �6    .+�5      �� -   5         ��Name$ SelectableCardsHack��5      $y�	Collapsed=�6[   ��63   �� *SelectableCards+6[   �� 5      $��0��6   $��1��6&    �� Orientation$ 
Horizontal=�66   $��Center=�6O   $��Center=�6j   .+�5!      �� $��Opponent mana:��6   �� 5"      -   5         ��Name$ P2Mana��5"      $��0,0,20,0q�6G   ��6(   �� *
P2ManaTest+6G   �� 5#      -   5        $ Player1Avatar��5#      $	��Top=�61   $��Stretch=�6F   ��5$      �� *OpponentAvatarTest+{�5%      ��  �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType" �	 �� Path$	 isPlayer1Turn���5&      �� 5'      
 �� 
TargetType"
 ��6    .�5(      ��  �� Property � |� �� Value 6:   ��  #  ElementToSelectabilityConverter �� 	Converter# [   �� ConverterParameter %  System.Windows.Markup.Reference     Name$ SelectableCardsHack���� 5)       � �� 5*   "   �� 5+   &    �� 
TargetType" ��66   .R�5,   *   ��  ��Name$ Bd��61   #��z��6=   #��z��6i   #��z��6�    �� Padding# z��6�    J� SnapsToDevicePixels . ._�5-   .   �� #��z��6?   # z 6�   #��z��6�   5.   +    �� Triggers 5/   *   P� 50   .    P� Property �@��
IsSelected P� Value . . �51   2   ��  �� 
TargetName$ Bd��6O    � ��
 �  ��6_   53   /   ]� 54   .    ]� 
Conditions 55   2   �� 56   6    �� Property �@��
IsSelected �� Value . 6V   �� 57   6    �@��IsSelectionActive .  6f   ./�59   2   �� $ Bd��6O    � ��
 �  ��6_   5;   /   P� 5<   .    � |� .  . �5=   2   ��  � ��
 ���� 6O   5>   /   P� 5?   .    � |� . . �5@   2   ��  � ��
 �����6O   5A   /   5C   '   5E      5F      5H      �� 5I      -   5        $ label2��5I      $��Opponent hitpoints:��6(   �� 5J      -   5        $ label3��5J      ��6(   �� *P2Hitpoints+ �� UpdateSourceTrigger$ PropertyChanged=�5K      �� 5L      $��1��6   $��0��6%   $��Opponent hand:��65   $��Center=�6K   �� 5M      $��2��6   $��0��6%   $��Opponent field:��65   $��Center=�6M   �� 5O      -   5        $ P1Hand��5O      $��1��6&   $��1��63   $��2q�6C   $��Stretch=�6N   $��Stretch=�5P      $
��Left=�68   $��Stretch=�6Z   ��5Q      �� *P1Hand+{�5R      ��  �� $ FindAncestor=�" �$	 isPlayer1Turn���5S      �� 5T      .R�5U      �  K� IsItemsHost . ��67   �� * (FrameworkElement.ActualWidth)+ �� $ ScrollContentPresenterL�5V      ��5X      �� 5Y      .R�5Z      �� $��Center=�6$   $��Center=�6?   $��10q�6\   ��6h   �� 5[      ��5^      �� 5_      "
 ��6   .�5`      ��  � |� 66   �� # [     $ SelectableCardsHack���� 5a       � �� 5b      �� 5c   "   " ��62   .R�5d   &   �� $ Bd��6-   #��z��69   #��z��6e   #��z��6�   # z��6�    . ._�5e   *   �� #��z��6;   # z 6~   #��z��6�   5f   '    5g   &   P� 5h   *    �@��
IsSelected . . �5i   .   �� $ Bd��6K    � ��
 �  ��6[   5k   +   ]� 5l   *    5m   .   �� 5n   2    �@��
IsSelected . 6R   �� 5o   2    �@��IsSelectionActive .  6b   ./�5q   .   �� $ Bd��6K    � ��
 �  ��6[   5s   +   P� 5t   *    � |� .  . �5u   .   ��  � ��
 ���� 6K   5v   +   P� 5w   *    � |� . . �5x   .   ��  � ��
 �����6K   5y   +   5{   #   5}      5~      5�      �� 5�      -   5        $ P1Field��5�      $��2��6'   $��1��64   $��2q�6D   $��Stretch=�6O   $��Stretch=�5�      $
��Left=�68   $��Center=�6Z   ��5�      �� *	P1Field+{�6:   ��  �� $ FindAncestor=�" �$	 isPlayer1Turn���5�      �� 5�      .R�5�      �  . ��67   �� * (FrameworkElement.ActualWidth)+ �� $ ScrollContentPresenterL�5�      ��5�      �� 5�      .R�5�      �� $��Center=�6$   $��Center=�6?   $��10q�6\   ��6h   �� 5�      ��5�      �� 5�      "
 ��6   .�5�      ��  � |� 66   �� # [     $ SelectableCardsHack���� 5�       � �� 5�      �� 5�   "   " ��62   .R�5�   &   �� $ Bd��6-   #��z��69   #��z��6e   #��z��6�   # z��6�    . ._�5�   *   �� #��z��6;   # z 6~   #��z��6�   5�   '    5�   &   P� 5�   *    �@��
IsSelected . . �5�   .   �� $ Bd��6K    � ��
 �  ��6[   5�   +   ]� 5�   *    5�   .   �� 5�   2    �@��
IsSelected . 6R   �� 5�   2    �@��IsSelectionActive .  6b   ./�5�   .   �� $ Bd��6K    � ��
 �  ��6[   5�   +   P� 5�   *    � |� .  . �5�   .   ��  � ��
 ���� 6K   5�   +   P� 5�   *    � |� . . �5�   .   ��  � ��
 �����6K   5�   +   5�   #   5�      5�      5�      �� 5�      $��5��6   $��1��6&   $ 
Horizontal=�66   $��Center=�6O   $��Center=�6j   .+�5�      �� $��My mana:��6   $
��Left=�6+   �� 5�      -   5        $ P1Mana��5�      $��0,0,20,0q�6G   ��6(   �� *
P1ManaTest+6G   �� 5�      -	   5        $ Player0Avatar��5�      $	��Top=�61   $��Stretch=�6F   ��5�      �� *Player0Avatar+{�6t   ��  �� $ FindAncestor=�" �$	 isPlayer0Turn���5�      �� 5�      "
 ��6    .�5�      ��  � |� 6:   �� # [     $ SelectableCardsHack���� 5�       � �� 5�   "   �� 5�   &   " ��66   .R�5�   *   �� $ Bd��61   #��z��6=   #��z��6i   #��z��6�   # z��6�    . ._�5�   .   �� #��z��6?   # z 6�   #��z��6�   5�   +    5�   *   P� 5�   .    �@��
IsSelected . . �5�   2   �� $ Bd��6O    � ��
 �  ��6_   5�   /   ]� 5�   .    5�   2   �� 5�   6    �@��
IsSelected . 6V   �� 5�   6    �@��IsSelectionActive .  6f   ./�5�   2   �� $ Bd��6O    � ��
 �  ��6_   5�   /   P� 5�   .    � |� .  . �5�   2   ��  � ��
 ���� 6O   5�   /   P� 5�   .    � |� . . �5�   2   ��  � ��
 �����6O   5�   /   5�   '   5�      5�      5�      �� 5�      -
   5        $ label��5�      $��My hitpoints:��6'   �� 5�      -   5        $ label1��5�      $��	0,0,100,0q�6m   ��6(   �� *P1Hitpoints+$ PropertyChanged=�6m   5�      �� 5�      $��3��6   $��0��6%   $��My hand:��65   $��Center=�6E   �� 5�      $��4��6   $��0��6%   $��	My field:��65   $��Center=�6G   �� 5�      -   5        $ P0Hand��5�      $��3��6&   $��1��63   $��2q�6C   $��Stretch=�6N   $��Stretch=�5�      $
��Left=�68   $��Stretch=�6Z   $��Disabled=�5�   �   $��Disabled=�6�   {�6   ��  �� $ FindAncestor=�" �$	 isPlayer0Turn���5�      �� *P0Hand+��5�      �� 5�      .R�5�      �  . ��67   �� * (FrameworkElement.ActualWidth)+ �� $ ScrollContentPresenterL�5�      ��5�      �� 5�      .R�5�      �� $��Center=�6$   $��Center=�6?   $��10q�6\   ��6h   �� 5�      ��5�      �� 5�      "
 ��6   .�5�      ��  � |� 66   �� # [     $ SelectableCardsHack���� 5�       � �� 5�      �� 5�   "   " ��62   .R�5�   &   �� $ Bd��6-   #��z��69   #��z��6e   #��z��6�   # z��6�    . ._�5�   *   �� #��z��6;   # z 6~   #��z��6�   5�   '    5�   &   P� 5   *    �@��
IsSelected . . �5  .   �� $ Bd��6K    � ��
 �  ��6[   5  +   ]� 5  *    5  .   �� 5  2    �@��
IsSelected . 6R   �� 5  2    �@��IsSelectionActive .  6b   ./�5	  .   �� $ Bd��6K    � ��
 �  ��6[   5  +   P� 5  *    � |� .  . �5  .   ��  � ��
 ���� 6K   5  +   P� 5  *    � |� . . �5  .   ��  � ��
 �����6K   5  +   5  #   5     5     5     �� 5     -   5        $ P0Field��5     $��4��6'   $��1��64   $��2q�6D   $��Stretch=�6O   $��Stretch=�5     $
��Left=�68   $��Center=�6Z   ��5     �� *	P0Field+{�6:   ��  �� $ FindAncestor=�" �$	 isPlayer0Turn���5     �� 5     .R�5     �  . ��67   �� * (FrameworkElement.ActualWidth)+ �� $ ScrollContentPresenterL�5     ��5!     �� 5"     .R�5#     �� $��Center=�6$   $��Center=�6?   $��10q�6\   ��6h   �� 5$     ��5&     �� 5'     "
 ��6   .�5(     ��  � |� 66   �� # [     $ SelectableCardsHack���� 5)      � �� 5*     �� 5+  "   " ��62   .R�5,  &   �� $ Bd��6-   #��z��69   #��z��6e   #��z��6�   # z��6�    . ._�5-  *   �� #��z��6;   # z 6~   #��z��6�   5.  '    5/  &   P� 50  *    �@��
IsSelected . . �51  .   �� $ Bd��6K    � ��
 �  ��6[   53  +   ]� 54  *    55  .   �� 56  2    �@��
IsSelected . 6R   �� 57  2    �@��IsSelectionActive .  6b   ./�59  .   �� $ Bd��6K    � ��
 �  ��6[   5;  +   P� 5<  *    � |� .  . �5=  .   ��  � ��
 ���� 6K   5>  +   P� 5?  *    � |� . . �5@  .   ��  � ��
 �����6K   5A  +   5C  #   5E     5F     5H     5I     � 5K  
   $	��Top=�6   $��	0,10,0,10q�6$    5L     �� 5M     �� 5N     �� 5O     �� 5P     �� 5Q     .+�5S     ]� $��0��6   $��	10,0,10,0q�6(   .+�5T     �� $	��Top=�6   $��Playable cards��61   $��Center=�6G   $��0,0,0,10q�6d   �� 5U     -   5        $ SelectableCards��5U     $	��Top=�63   $��Stretch=�6H   ��5V     �� *SelectableCards+6B   5W     ]� 5X     $��1��6   $��	10,0,10,0q�6(   .+�5Y     �� $	��Top=�6   $��Card details��61   $��Center=�6E   $��0,0,0,10q�6b   �� 5Z     -   5        $ MyCard��5Z     $	��Top=�6(   ��5[     ��  �� ElementName$ SelectableCards��$	 BorderBrush���5\     �� $ SelectableCards��$	 BorderThickness���5]     �� $ SelectableCards��$	 SelectedValue�$ PropertyChanged=�  CardToStringConverter# [ 5^     ]� 5_     $��2��6   $��	10,0,10,0q�6(   .+�5`     �� $	��Top=�6   $��Possible targets��61   $��Center=�6I   $��0,0,0,10q�6f   �� 5a     -   5        $ ValidTargets��5a     $	��Top=�60   $��Stretch=�6E   ��5b     �� *ValidTargets+6?   5c     ]� 5d     $��3��6   $��	10,0,10,0q�6(   .+�5e     �� $	��Top=�6   $��Target details��61   $��Center=�6G   $��0,0,0,10q�6d   �� 5f     -   5        $ MyCardFromField��5f     $	��Top=�61   ��5g     �� $ SelectableCards��$	 BorderBrush���5h     �� $ SelectableCards��$	 BorderThickness���5i     �� $ ValidTargets��$	 SelectedValue�$ PropertyChanged=�# [ 5j     ]� 5k     $��4��6   $��	10,0,10,0q�6(   .+�5l     �� -   5         ��Name$ Confirm��5l  �   $	��Top=�6   $��	0,40,0,10q�6.   $	��150��6A   $��70��6M   $��Center=�6Y   $��Center=�6v   $��Confirm��6�   {�.  6�   �� 5m     -   5        $ EOTbutt��5m  ~   $	��Top=�6   $	��150��6.   $��70��6:   $��Center=�6F   $��Center=�6c   $��END TURN��6�   6�   �� 5n     -   5        $	��Top=�5n     $��0,0,0,10q�6.   $	��150��6@   $��70��6L   $��Center=�6X   $��Bottom=�6u   $
��Quit��6�   6�   5o     5p     5q     5r     