//
//  IOSCustomEmoji.h
//  Unity-iPhone
//
//  Created by akb on 2018/12/19.
//

#ifndef IOSCustomEmoji_h
#define IOSCustomEmoji_h

#import <UIKit/UIKit.h>
#endif /* IOSCustomEmoji_h */
@interface IOSCustomEmoji : UIViewController<UIImagePickerControllerDelegate,UINavigationControllerDelegate>
{
 
}
-(void) openMenu;
+(void) saveImageToPhotosAlbum : (NSString*) p1;
@end
