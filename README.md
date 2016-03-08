# XamAndroidAwaitBug
Example project demonstrating bug in async/await on Xamarin Android for OSX

## Repro steps

Build and run, click the button to test. After "getting page..." the button will change to either "Corrupted async result" or "Success", assuming your internet works

Build in Xamarin Studio Mac and you will see "Corrupted async result" after clicking the button.
Build in Visual Studio or Xamarin Studio on Windows and you will see "Success"

Comment out `#define FAIL_TEST` in MyClient.cs and rebuild on XS Mac. Now you will see "Success"
