# TstBrowserWinUI3

This is a **Test Browser made with WinUi3**. 

## Requirements

You will need [Visual studio](https://visualstudio.microsoft.com/downloads/)(Cannot use VS code) to compile the codes. See [Tutorial to use winui3](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=cs-vs-community%2Ccpp-vs-community%2Cvs-2022-17-1-a%2Cvs-2022-17-1-b).

This app is written and built in Windows 11. Though most of the functions in winui3 works on Windows 10, it is not guaranteed that the app will work properly on Windows 10

## Codes

### App

### MainWindow
Provides a window with TabView in it. Stores some cross-page properties(eg Is setting page opened)

### TabPage
Things inside each tabs goes here. Will be created as a new and independent tab every time you add a new tab.