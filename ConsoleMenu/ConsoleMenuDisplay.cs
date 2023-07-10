﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTools;

internal sealed class ConsoleMenuDisplay
{
  private readonly IConsole console;
  private readonly ItemsCollection menuItems;
  private readonly List<string> titles;
  private readonly MenuConfig config;
  private readonly VisibilityManager visibility;
  private readonly CloseTrigger closeTrigger;
  private readonly string noSelectorLine;

  public ConsoleMenuDisplay(
    ItemsCollection menuItems,
    IConsole console,
    List<string> titles,
    MenuConfig config,
    CloseTrigger closeTrigger)
  {
    this.menuItems = menuItems;
    this.console = console;
    this.titles = titles;
    this.config = config;
    this.visibility = new VisibilityManager(menuItems.Items.Count);
    this.closeTrigger = closeTrigger;
    this.noSelectorLine = new string(' ', this.config.Selector.Length);
  }

  public async Task ShowAsync(CancellationToken token)
  {
    var selectedItem = this.menuItems.GetSeletedItem();
    if (selectedItem != null)
    {
      await selectedItem.AsyncAction.Invoke(token);
      return;
    }

    ConsoleKeyInfo key;
    this.menuItems.ResetCurrentIndex();
    var currentForegroundColor = this.console.ForegroundColor;
    var currentBackgroundColor = this.console.BackgroundColor;
    bool breakIteration = false;
    var filter = new StringBuilder();

    while (true)
    {
      token.ThrowIfCancellationRequested();
      do
      {
        if (this.config.ClearConsole)
        {
          this.console.Clear();
        }

        if (this.config.EnableBreadcrumb)
        {
          this.config.WriteBreadcrumbAction(this.titles);
        }

        if (this.config.EnableWriteTitle)
        {
          this.config.WriteTitleAction(this.config.Title);
        }

        this.config.WriteHeaderAction();

        foreach (var menuItem in this.menuItems.Items)
        {
          if (this.config.EnableFilter && !this.visibility.IsVisibleAt(menuItem.Index))
          {
            this.menuItems.SelectClosestVisibleItem(this.visibility);
          }
          else
          {
            this.WriteLineWithItem(menuItem);
          }
        }

        if (breakIteration)
        {
          breakIteration = false;
          break;
        }

        this.WriteLineWithButtons(this.menuItems.Buttons);

        if (this.config.EnableFilter)
        {
          this.console.Write(this.config.FilterPrompt + filter);
        }

        readKey:
        key = this.console.ReadKey(true);

        if (key.Key == ConsoleKey.DownArrow)
        {
          this.menuItems.SelectNextVisibleItem(this.visibility);
        }
        else if (key.Key == ConsoleKey.UpArrow)
        {
          this.menuItems.SelectPreviousVisibleItem(this.visibility);
        }
        else if (this.menuItems.CanSelectButton(key.KeyChar))
        {
          this.menuItems.SelectButton(key.KeyChar);
          breakIteration = true;
        }
        else if (this.menuItems.CanSelect(key.KeyChar))
        {
          this.menuItems.Select(key.KeyChar);
          breakIteration = true;
        }
        else if (key.Key != ConsoleKey.Enter)
        {
          if (this.config.EnableFilter)
          {
            if (key.Key == ConsoleKey.Backspace)
            {
              if (filter.Length > 0)
              {
                filter.Length--;
              }
            }
            else if (!char.IsControl(key.KeyChar))
            {
              filter.Append(key.KeyChar);
            }

            var filterString = filter.ToString();

            this.visibility.SetVisibleWithPredicate(this.menuItems.Items,
              (item) => item.Name.Contains(filterString, StringComparison.OrdinalIgnoreCase));
          }
          else
          {
            goto readKey;
          }
        }
      } while (key.Key != ConsoleKey.Enter);

      this.console.WriteLine();
      this.console.ForegroundColor = currentForegroundColor;
      this.console.BackgroundColor = currentBackgroundColor;

      try
      {
        var action = this.menuItems.CurrentButton?.AsyncAction ?? this.menuItems.CurrentItem.AsyncAction;
        if (action == ConsoleMenu.Close)
        {
          this.menuItems.UnsetSelectedIndex();
          return;
        }
        else
        {
          await action(token).ConfigureAwait(false);
          if (this.closeTrigger.IsOn())
          {
            this.menuItems.UnsetSelectedIndex();
            this.closeTrigger.SetOff();
            return;
          }
        }
      }
      catch (ArgumentOutOfRangeException ex)
      {
        this.console.WriteLine("No item selected");
      }
    }
  }

  private void WriteLineWithItem(MenuItem menuItem)
  {
    if (this.menuItems.IsSelected(menuItem))
    {
      this.console.BackgroundColor = menuItem.SelectedItemBackgroundColor ?? this.config.SelectedItemBackgroundColor;
      this.console.ForegroundColor = menuItem.SelectedItemForegroundColor ?? this.config.SelectedItemForegroundColor;
      this.console.Write(this.config.Selector);
    }
    else
    {
      this.console.BackgroundColor = menuItem.ItemBackgroundColor ?? this.config.ItemBackgroundColor;
      this.console.ForegroundColor = menuItem.ItemForegroundColor ?? this.config.ItemForegroundColor;
      this.console.Write(this.noSelectorLine);
    }

    this.config.WriteItemAction(menuItem);
    this.console.WriteLine();
    this.console.BackgroundColor = this.config.ItemBackgroundColor;
    this.console.ForegroundColor = this.config.ItemForegroundColor;
  }

  private void WriteLineWithButtons(Dictionary<char, MenuItem> buttons)
  {
    foreach (var pair in buttons)
    {
      var item = pair.Value;
      this.console.BackgroundColor = item.ItemBackgroundColor ?? this.config.ItemBackgroundColor;
      this.console.ForegroundColor = item.ItemForegroundColor ?? this.config.ItemForegroundColor;
      this.console.Write($"[{pair.Key}] {item.Name}");
      this.console.BackgroundColor = this.config.ItemBackgroundColor;
      this.console.ForegroundColor = this.config.ItemForegroundColor;
      this.console.Write("   ");
    }

    this.console.WriteLine();
  }
}
