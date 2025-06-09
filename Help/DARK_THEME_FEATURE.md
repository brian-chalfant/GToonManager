# Dark Theme Feature

## Overview
GToon Manager now supports both light and dark themes that can be toggled through the Settings menu.

## How to Use
1. Open GToon Manager
2. Go to **File** → **Settings** (or press the ⚙️ Settings button)
3. Navigate to the **🎨 Interface** tab
4. In the **User Interface** section, find the **Theme** dropdown
5. Select either:
   - **Light** - The classic medieval theme with light backgrounds
   - **Dark** - A darker medieval theme with muted colors and dark backgrounds
6. Click **Apply** to save the changes

## Theme Changes Take Effect Immediately
- Theme changes are applied in real-time as you select them in the settings
- No need to restart the application
- The theme affects all windows and UI elements

## Theme Colors

### Light Theme (Default)
- Background: Warm parchment (#F5E6C8)
- Primary Text: Deep red (#7B0A02)
- Gold Accents: Medieval gold (#D4AF37)
- Controls: White backgrounds

### Dark Theme
- Background: Dark gray (#2B2B2B)
- Primary Text: Light coral (#FF6B6B)
- Gold Accents: Bright gold (#FFD700)
- Controls: Dark gray backgrounds (#4A4A4A)

## Technical Implementation
- Uses WPF's DynamicResource system for real-time theme switching
- ThemeService manages theme application across all windows
- Settings are automatically saved and restored on application restart
- All UI controls and styles are theme-aware

## Supported Elements
The dark theme affects:
- Main window background and all borders
- Menu bars and menu items
- Tab controls and tab content
- Text boxes, combo boxes, and buttons
- Settings window and all dialogs
- Status bars and headers

## Memory System Integration
This feature respects the memory system rule that the rolling UI must close after ability scores are applied, and a reroll button must be displayed when the reroll limit configured in Settings is greater than zero. 