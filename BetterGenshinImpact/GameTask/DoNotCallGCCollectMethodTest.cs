



// Reference GitHub Links:
// Rule: "GC.Collect" should not be called
// Repository: https://github.com/vcatest/OrchardCore
//   - https://github.com/vcatest/OrchardCore/blob/master/src/OrchardCore.Modules/OrchardCore.Demo/Controllers/HomeController.cs#L125
// Repository: https://github.com/vcatest/Playnite.git
//   - https://github.com/vcatest/Playnite/blob/master/source/Playnite/Common/MemoryCache.cs#L74
//   - https://github.com/vcatest/Playnite/blob/master/source/Playnite/Controls/FadeImage.xaml.cs#L202
// Repository: https://github.com/vcatest/PowerToys
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/MouseUtils/MouseJumpUI/MainForm.cs#L222
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/MouseWithoutBorders/App/Core/Clipboard.cs#L114
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/MouseWithoutBorders/App/Helper/FormHelper.cs#L350
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/PowerOCR/PowerOCR/Helpers/ImageMethods.cs#L152
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/PowerOCR/PowerOCR/Helpers/ImageMethods.cs#L220
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/PowerOCR/PowerOCR/Helpers/ImageMethods.cs#L240
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/PowerOCR/PowerOCR/Helpers/ImageMethods.cs#L260
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/PowerOCR/PowerOCR/Helpers/WindowUtilities.cs#L66
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/cmdpal/Microsoft.CmdPal.UI/ExtViews/ListPage.xaml.cs#L107
//   - https://github.com/vcatest/PowerToys/blob/master/src/modules/previewpane/common/controls/FormHandlerControl.cs#L110
// Repository: https://github.com/vcatest/ScreenToGif.git
//   - https://github.com/vcatest/ScreenToGif/blob/master/Other/Translator/ExceptionViewer.xaml.cs#L49
//   - https://github.com/vcatest/ScreenToGif/blob/master/Other/Translator/TranslatorWindow.xaml.cs#L378
//   - https://github.com/vcatest/ScreenToGif/blob/master/Other/Translator/TranslatorWindow.xaml.cs#L429
//   - https://github.com/vcatest/ScreenToGif/blob/master/Other/Translator/TranslatorWindow.xaml.cs#L669
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/Codification/PixelUtil.cs#L169
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/Codification/PixelUtil.cs#L179
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/Codification/PixelUtil.cs#L222
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/LocalizationHelper.cs#L138
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/MutexList.cs#L12
//   - https://github.com/vcatest/ScreenToGif/blob/master/ScreenToGif.Util/MutexList.cs#L45

using System;
using System.Drawing; // Simulating Bitmap dependency
using System.Threading;

namespace TestSources
{
    /// <summary>
    /// Tests for DoNotCallGCCollectMethod rule
    /// This rule detects calls to GC.Collect() and GC.GetTotalMemory(true)
    /// which force garbage collection and can have significant performance impacts.
    /// </summary>
    public class DoNotCallGCCollectMethodTest
    {
        // ========== POSITIVE TESTS - Should Detect ==========

        public void TestGCCollectNoArgs()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect();
        }

        public void TestGCCollectWithGeneration()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect(0);
        }

        public void TestGCCollectWithGenerationAndMode()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect(0, GCCollectionMode.Forced);
        }

        public void TestGCCollectWithAllParameters()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect(0, GCCollectionMode.Forced, true);
        }

        public void TestGCGetTotalMemoryWithTrue()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            long memory = GC.GetTotalMemory(true);
        }

        public void TestGCGetTotalMemoryWithVariable()
        {
            bool forceFullCollection = true;
            // EMB-ISSUE: DoNotCallGCCollectMethod
            long memory = GC.GetTotalMemory(forceFullCollection);
        }

        public void TestGCCollectInConditional()
        {
            if (DateTime.Now.Hour > 12)
            {
                // EMB-ISSUE: DoNotCallGCCollectMethod
                GC.Collect();
            }
        }

        public void TestMultipleCallsFirst()
        {
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect();
            Console.WriteLine("After GC");
        }

        public void TestMultipleCallsSecond()
        {
            Console.WriteLine("Before GC");
            // EMB-ISSUE: DoNotCallGCCollectMethod
            GC.Collect(2);
        }

        // ========== NEGATIVE TESTS - Should NOT Detect ==========

        // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
        public void TestGCGetTotalMemoryWithFalse()
        {
            // GC.GetTotalMemory(false) is acceptable as it doesn't force collection
            long memory = GC.GetTotalMemory(false);
        }

        // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
        public void TestOtherGCMethods()
        {
            // These other GC methods are acceptable
            int gen = GC.GetGeneration(this);
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }

        // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
        public void TestCustomGCClass()
        {
            // Custom class named GC should not trigger the rule
            var customGC = new CustomGC();
            customGC.Collect();
        }

        // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
        public void TestNoGCCalls()
        {
            // No GC calls at all
            Console.WriteLine("No GC methods called here");
            int x = 42;
        }

        // -------------------------------------------------------------------------
        // SCENARIO: PowerToys - PowerOCR Image Methods
        // Reference: src/modules/PowerOCR/PowerOCR/Helpers/ImageMethods.cs
        // Description: Simulating heavy image processing where developers forcefully 
        // call GC.Collect() to reclaim unmanaged Bitmap memory immediately.
        // -------------------------------------------------------------------------
        public class ImageMethods
        {
            public void GetImageText(string imagePath)
            {
                Bitmap bmp = null;
                try
                {
                    bmp = new Bitmap(imagePath);
                    // Process image...
                }
                finally
                {
                    bmp?.Dispose();
                    
                    // Violation: Forcing GC after disposing large bitmaps
                    // EMB-ISSUE: DoNotCallGCCollectMethod
                    GC.Collect();
                    
                    // Violation: Waiting for finalizers specifically
                    GC.WaitForPendingFinalizers();
                    
                    // Violation: Second collect
                    // EMB-ISSUE: DoNotCallGCCollectMethod
                    GC.Collect();
                }
            }

            public void CompliantImageProcessing(string imagePath)
            {
                // Compliant: Relying on using statement (IDisposable) without forcing GC
                using (var bmp = new Bitmap(imagePath))
                {
                    // Process...
                }
                // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
                // No explicit GC call here
            }
        }

        // -------------------------------------------------------------------------
        // SCENARIO: PowerToys - Preview Pane Form Handler
        // Reference: src/modules/previewpane/common/controls/FormHandlerControl.cs
        // Description: UI Controls forcing GC during Unload/Dispose to handle COM interop or preview handlers.
        // -------------------------------------------------------------------------
        public class FormHandlerControl : IDisposable
        {
            public void Unload()
            {
                this.Dispose();
                
                // Violation: Forcing GC during control unload
                // EMB-ISSUE: DoNotCallGCCollectMethod
                GC.Collect();
            }

            public void Dispose()
            {
                // Dispose resources
            }
        }

        // -------------------------------------------------------------------------
        // SCENARIO: ScreenToGif - Translator & Localization
        // Reference: Other/Translator/TranslatorWindow.xaml.cs
        // Description: Applications clearing memory when switching large resource dictionaries or languages.
        // -------------------------------------------------------------------------
        public class TranslatorWindow
        {
            public void ImportTranslation()
            {
                // Import logic...
                
                // Violation: Garbage collection triggered after UI operation
                // EMB-ISSUE: DoNotCallGCCollectMethod
                GC.Collect(2, GCCollectionMode.Forced);
            }

            public void ChangeLanguage(string cultureCode)
            {
                // Switch dictionary...

                // Violation: Checking memory and forcing collection if needed
                // EMB-ISSUE: DoNotCallGCCollectMethod
                if (GC.GetTotalMemory(true) > 1024 * 1024 * 100)
                {
                    // Log high memory usage
                }
            }
        }

        // -------------------------------------------------------------------------
        // SCENARIO: ScreenToGif - PixelUtil & Codification
        // Reference: ScreenToGif.Util/Codification/PixelUtil.cs
        // -------------------------------------------------------------------------
        public class PixelUtil
        {
            public void ClearPixels(byte[] pixels)
            {
                Array.Clear(pixels, 0, pixels.Length);
                
                // Violation: Forcing GC after clearing large array
                // EMB-ISSUE: DoNotCallGCCollectMethod
                GC.Collect();
            }
        }

        // -------------------------------------------------------------------------
        // SCENARIO: Edge Cases
        // -------------------------------------------------------------------------
        public class EdgeCases
        {
            // Violation: Calling GC.Collect inside a property getter
            public int MemoryStatus
            {
                get
                {
                    // EMB-ISSUE: DoNotCallGCCollectMethod
                    GC.Collect();
                    return 0;
                }
            }

            // Violation: Calling inside a Finalizer (Very bad practice, but should be caught)
            ~EdgeCases()
            {
                // EMB-ISSUE: DoNotCallGCCollectMethod
                GC.Collect();
            }

            // Compliant: Calling WaitForPendingFinalizers alone (if not coupled with Collect, it's just a wait)
            // While often used with Collect, rule specifically targets Collect or GetTotalMemory(true).
            public void CleanUp()
            {
                // EMB-ISSUE: DoNotCallGCCollectMethod/no-detect
                GC.WaitForPendingFinalizers();
            }
        }

        // ========== HELPER CLASSES ==========

        private class CustomGC
        {
            public void Collect()
            {
                // Custom implementation, not System.GC
            }

            public long GetTotalMemory(bool forceFullCollection)
            {
                // Custom implementation, not System.GC
                return 0;
            }
        }
        
        // Mocking Bitmap for the test without System.Drawing reference
        public class Bitmap : IDisposable
        {
            public Bitmap(string path) { }
            public void Dispose() { }
        }
    }
}
