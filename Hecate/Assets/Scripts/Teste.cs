using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Teste : MonoBehaviour
{
    [TitleGroup("Tabs")]
    [HorizontalGroup("Tabs/Split", Width = 0.5f)]
    [TabGroup("Tabs/Split/Parameters", "A")]
    public string NameA, NameB, NameC;

    [TabGroup("Tabs/Split/Parameters", "B")]
    public int ValueA, ValueB, ValueC;

    [TabGroup("Tabs/Split/Buttons", "Responsive")]
    [ResponsiveButtonGroup("Tabs/Split/Buttons/Responsive/ResponsiveButtons")]
    public void Hello() { }

    [ResponsiveButtonGroup("Tabs/Split/Buttons/Responsive/ResponsiveButtons")]
    public void World() { }

    [ResponsiveButtonGroup("Tabs/Split/Buttons/Responsive/ResponsiveButtons")]
    public void And() { }

    [ResponsiveButtonGroup("Tabs/Split/Buttons/Responsive/ResponsiveButtons")]
    public void Such() { }

    [Button]
    [TabGroup("Tabs/Split/Buttons", "More Tabs")]
    [TabGroup("Tabs/Split/Buttons/More Tabs/SubTabGroup", "A")]
    public void SubButtonA() { }

    [Button]
    [TabGroup("Tabs/Split/Buttons/More Tabs/SubTabGroup", "A")]
    public void SubButtonB() { }

    [Button(ButtonSizes.Gigantic)]
    [TabGroup("Tabs/Split/Buttons/More Tabs/SubTabGroup", "B")]
    public void SubButtonC() { }
}
