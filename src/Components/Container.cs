using System.Windows.Controls;
using System.Windows;

namespace ActivAndZen.Components;

public partial class Container : Border
{
    Grid m_grid;
    public Container()
    {
        this.m_grid = new Grid();
    }
}
