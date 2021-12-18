/**
 * Hexa Snap
 * Copyright © 2017 Aurélien Lubecki
 * All Rights Reserved
 */

#import "NativeShareManager.h"
#import "NativePopupManager.h"
#import "NativeUtils.h"

@implementation NativeShareManager

+ (void) shareWithChooserTitle:(NSString*)chooserTitle subject:(NSString*)subject message:(NSString*)message gameUrl:(NSString*)gameUrl imagePath:(NSString*)imagePath  {
    
    UIImage* image = [UIImage imageWithContentsOfFile:imagePath];
    
    NSMutableArray* items = [NSMutableArray array];
    if (message != nil) {
        [items addObject:message];
    }
    if (gameUrl != nil) {
        [items addObject:gameUrl];
    }
    if (image != nil) {
        [items addObject:image];
    }
    
    UIActivityViewController* vc = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
    [NativeUtils presentModalViewController:vc];
}

@end

extern "C" {
    
    void call_share(const char* chooserTitle, const char* subject, const char* message, const char* gameUrl, const char* imagePath) {
        
        [NativeShareManager shareWithChooserTitle:[NativeUtils getNSString:chooserTitle]
                                          subject:[NativeUtils getNSString:subject]
                                          message:[NativeUtils getNSString:message]
                                          gameUrl:[NativeUtils getNSString:gameUrl]
                                        imagePath:[NativeUtils getNSString:imagePath]];
    }
    
}

