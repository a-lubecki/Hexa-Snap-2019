/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

#import "NativePopupManager.h"
#import "NativeUtils.h"
#import "UnityAppController.h"

@implementation NativePopupManager

+ (void) dismissAlertController {
    
    UIViewController* mainVc = NativeUtils.getMainViewController;
    UIViewController* vc = mainVc.presentedViewController;
    
    if (vc != nil && [vc isKindOfClass:UIAlertController.class]) {
        [mainVc dismissViewControllerAnimated:YES completion:nil];
    }
    
}

+ (void) callNativeCallback:(NSInteger)buttonPos {
    
    [NativePopupManager dismissAlertController];
    
    UnitySendMessage("MainScriptsManager", "onPopupButtonClick", [NSString stringWithFormat:@"%ld", (long)buttonPos].UTF8String);
}

+ (void) showAlertDialogWithTitle:(NSString*)title message:(NSString*)message negativeButtonText:(NSString*)negativeButtonText positiveButtonText:(NSString*)positiveButtonText {
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title message:message preferredStyle:UIAlertControllerStyleAlert];
    
    if (positiveButtonText != nil && positiveButtonText.length > 0) {
	
		[alert addAction:[UIAlertAction actionWithTitle:positiveButtonText style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
			
			[NativePopupManager callNativeCallback:0];
			
		}]];
	}
    
    if (negativeButtonText == nil || negativeButtonText.length <= 0) {
        negativeButtonText = @"Ok";
    }
    
    [alert addAction:[UIAlertAction actionWithTitle:negativeButtonText style:UIAlertActionStyleCancel handler:^(UIAlertAction * _Nonnull action) {
        
        [NativePopupManager callNativeCallback:-1];
        
    }]];
    
    [NativePopupManager dismissAlertController];
    
    [NativeUtils.getMainViewController presentViewController:alert animated:YES completion:nil];
}

+ (void) showActionSheetDialogWithTitle:(NSString*)title message:(NSString*)message negativeButtonText:(NSString*)negativeButtonText buttonTexts:(NSArray*)buttonTexts {
    
    if (title != nil && title.length <= 0) {
        title = nil;
    }
    
    if (message != nil && message.length <= 0) {
        message = nil;
    }
    
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:title message:message preferredStyle:UIAlertControllerStyleActionSheet];
    
    for (int i = 0 ; i < buttonTexts.count ; i++) {
        
        NSString* text = buttonTexts[i];
        
        [alert addAction:[UIAlertAction actionWithTitle:text style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            
            [NativePopupManager callNativeCallback:i];
            
        }]];
    }
    
    if (negativeButtonText == nil || negativeButtonText.length <= 0) {
        negativeButtonText = @"Ok";
    }
    
    [alert addAction:[UIAlertAction actionWithTitle:negativeButtonText style:UIAlertActionStyleCancel handler:^(UIAlertAction * _Nonnull action) {
        
        [NativePopupManager callNativeCallback:-1];
        
    }]];
    
    [NativePopupManager dismissAlertController];
    
    //fix ipad display
    alert.modalPresentationStyle = UIModalPresentationPopover;
    
    [NativeUtils presentModalViewController:alert];
}

@end

extern "C" {
    
    void call_showAlertDialog(const char* title, const char* message, const char* negativeButtonText, const char* positiveButtonText) {
        
        [NativePopupManager showAlertDialogWithTitle:[NativeUtils getNSString:title]
                                             message:[NativeUtils getNSString:message]
                                  negativeButtonText:[NativeUtils getNSString:negativeButtonText]
                                  positiveButtonText:[NativeUtils getNSString:positiveButtonText]];
        
    }
    
    void call_showActionSheetDialog(const char* title, const char* message, const char* negativeButtonText, const char* buttonTexts[], int buttonTextsCount) {
        
        [NativePopupManager showActionSheetDialogWithTitle:[NativeUtils getNSString:title]
                                                   message:[NativeUtils getNSString:message]
                                        negativeButtonText:[NativeUtils getNSString:negativeButtonText]
                                               buttonTexts:[NativeUtils getNSArrayOfNSString:buttonTexts count:buttonTextsCount]];
    }
    
}

