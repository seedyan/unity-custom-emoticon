//
//  IOSCustomEmoji.m
//  Unity-iPhone
//
//  Created by akb on 2018/12/19.
//

#import "IOSCustomEmoji.h"
#import <AVFoundation/AVFoundation.h>
#import <Photos/PHPhotoLibrary.h>

@implementation IOSCustomEmoji
#if defined (__cplusplus)
extern "C"
{
#endif
    //打开菜单
    void _openPicture()
    {
        NSLog(@"_openPicture");
        IOSCustomEmoji *app = [[IOSCustomEmoji alloc] init];
        UIViewController *viewController = UnityGetGLViewController();
        [viewController.view addSubview:app.view];
        [app openMenu];
    }
    
    //保存图片
    void _saveImageToPhotoAlbum(char* addr)
    {
        NSLog(@"_saveImageToPhotoAlbum");
        NSString *value = [NSString stringWithUTF8String:addr];
        [IOSCustomEmoji saveImageToPhotosAlbum:value];
    }
#if defined (__cplusplus)
}
#endif

-(void)openMenu
{
    UIAlertController *actionSheet = [UIAlertController alertControllerWithTitle:@"选择图像" message:nil preferredStyle:UIAlertControllerStyleActionSheet];

    
    // 创建action，这里action1只是方便编写，以后再编程的过程中还是以命名规范为主
    UIAlertAction *action1 = [UIAlertAction actionWithTitle:@"拍照" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [self takePhoto];
        NSLog(@"打开照相机拍照");
    }];
    UIAlertAction *action2 = [UIAlertAction actionWithTitle:@"从相册选择" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [self LocalPhoto];
        NSLog(@"打开相册");
    }];
    UIAlertAction *action3 = [UIAlertAction actionWithTitle:@"取消" style:UIAlertActionStyleCancel handler:^(UIAlertAction * _Nonnull action) {
        NSLog(@"cancle");
    }];
    
    //把action添加到actionSheet里
    [actionSheet addAction:action1];
    [actionSheet addAction:action2];
    [actionSheet addAction:action3];
    NSLog(@"添加完成");
    
    //相当于之前的[actionSheet show];
    //ipad 打开窗口方式
    if([self getIsIpad])
    {
        UIPopoverPresentationController * popPresenter = [actionSheet popoverPresentationController];
        popPresenter.sourceView = self.view;
        
        popPresenter.sourceRect = self.view.frame;
        //popPresenter.permittedArrowDirections = UIPopoverArrowDirectionUnknown;
        //隐藏尖头
        [actionSheet.popoverPresentationController setPermittedArrowDirections:0];
        
        [self presentViewController:actionSheet animated:YES completion:nil];
    }
    else
    {
        [self presentViewController:actionSheet animated:YES completion:nil];
    }
}

//开始拍照
-(void)takePhoto
{
    NSString *mediaType = AVMediaTypeVideo;//读取媒体类型
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:mediaType];//读取设备授权状态
    if(authStatus == AVAuthorizationStatusRestricted || authStatus == AVAuthorizationStatusDenied){
        
        NSString *errorStr = @"请在iPhone的“设置-隐私-相机”选项中，允许访问你的相机。";
        [self showAlertMessage:errorStr];
    }
    else
    {
        if ([self isCameraAvailable] &&([self isFrontCameraAvailable] || [self isRearCameraAvailable]))
        {
            UIImagePickerController *picker = [[UIImagePickerController alloc] init];
            NSLog(@"%@",self);
            picker.delegate = self;
            //设置拍照后的图片可被编辑
            picker.allowsEditing = NO;
            picker.sourceType = UIImagePickerControllerSourceTypeCamera;
            [self presentViewController:picker animated:YES completion:nil];
        }else
        {
            NSLog(@"相机有问题");
        }
    }
}

//打开本地相册
-(void)LocalPhoto
{
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    if (status == PHAuthorizationStatusRestricted || status == PHAuthorizationStatusDenied)
    {
        NSString *errorStr = @"请在iPhone的“设置-隐私-相册”选项中，允许访问你的相册。";
        [self showAlertMessage:errorStr];
    }
    else
    {
        UIImagePickerController *picker = [[UIImagePickerController alloc] init];
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        picker.delegate = self;
        //设置选择后的图片可被编辑
        picker.allowsEditing = NO;
        if([self getIsIpad])
        {
            UIPopoverController *pop=[[UIPopoverController alloc]initWithContentViewController:picker];
            [pop presentPopoverFromRect: CGRectMake(0, self.view.bounds.size.height, 0, 0)
                                 inView:self.view
               permittedArrowDirections:0
                               animated:YES];
        }
        else
        {
            [self presentViewController:picker animated:YES completion:nil];
        }

        //[[[[UIApplication sharedApplication] keyWindow] rootViewController] presentViewController:picker animated:YES completion:nil];

    }
}

// 判断设备是否有摄像头
- (BOOL) isCameraAvailable{
    return [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
}

// 前面的摄像头是否可用
- (BOOL) isFrontCameraAvailable{
    return [UIImagePickerController isCameraDeviceAvailable:UIImagePickerControllerCameraDeviceFront];
}

// 后面的摄像头是否可用
- (BOOL) isRearCameraAvailable{
    return [UIImagePickerController isCameraDeviceAvailable:UIImagePickerControllerCameraDeviceRear];
}

- (void) showAlertMessage:(NSString *) myMessage  {
    //创建提示框指针
    UIAlertController *alertMessage;
    //用参数myMessage初始化提示框
    alertMessage = [UIAlertController alertControllerWithTitle:@"提示" message:myMessage preferredStyle:UIAlertControllerStyleAlert];
    //添加按钮
    [alertMessage addAction:[UIAlertAction actionWithTitle:@"以后再说" style:UIAlertActionStyleCancel handler:nil]];
    //添加按钮
    [alertMessage addAction:[UIAlertAction actionWithTitle:@"立即前往" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }]];
    //display the message on screen  显示在屏幕上
    [[[[UIApplication sharedApplication] keyWindow] rootViewController] presentViewController:alertMessage animated:YES completion:nil];
}


//当选择一张图片后进入这里
-(void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary<UIImagePickerControllerInfoKey, id> *)info

{
    NSLog(@"当选择一张图片后进入这里");
    [picker dismissViewControllerAnimated:YES completion:nil];
    NSString *type = [info objectForKey:UIImagePickerControllerMediaType];
    
    //当选择的类型是图片
    if ([type isEqualToString:@"public.image"])
    {
        //先把图片转成NSData
        UIImage* image = [info objectForKey:@"UIImagePickerControllerOriginalImage"];
        if(image == nil)
        {
            NSLog(@"返回的image==null");
            return;
        }
        
        //判断图片的b分辨率是否支持
        Boolean SizeIsOk = [self CheckOutImageSize:image];
        if(!SizeIsOk)
        {
            NSString * result = @"图片分辨率不支持";
            UnitySendMessage( "SDKCallBackObj", "CommonTip", result.UTF8String);
            return;
        }
        
        //压缩图片
        image = [self imageWithImageSimple:image];
        NSData *imgData;
        if(UIImagePNGRepresentation(image) == nil)
        {
            imgData= UIImageJPEGRepresentation(image, 1);
        }
        else
        {
            imgData= UIImagePNGRepresentation(image);
        }
        NSString *base64ImageStr = [imgData base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
        NSLog(@"base64:%@",base64ImageStr);
        UnitySendMessage("SDKCallBackObj", "GetBase64" , [base64ImageStr UTF8String]);
    }
    else
    {
        NSLog(@"%@",type);
    }
}
// 打开相册后点击“取消”的响应
- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    NSLog(@" --- imagePickerControllerDidCancel !!");
    [picker dismissViewControllerAnimated:YES completion:nil];
}

//压缩图片
- (UIImage*)imageWithImageSimple:(UIImage*)image
{
    NSLog(@"压缩图片");
    NSLog(@"old h=%f",image.size.height);
    NSLog(@"old w=%f",image.size.width);
    CGFloat max = fmax(image.size.height, image.size.width);
    if(max<=1000)
        return image;
    else
    {
        CGFloat value = 1000 / max;
        
        CGSize newSize= CGSizeMake(image.size.width * value, image.size.height * value);
        // Create a graphics image context
        UIGraphicsBeginImageContext(newSize);
        
        // Tell the old image to draw in this new context, with the desired
        // new size
        [image drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
        
        // Get the new image from the context
        UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
        
        // End the context
        UIGraphicsEndImageContext();
        
        NSLog(@"new h=%f",newImage.size.height);
        NSLog(@"new w=%f",newImage.size.width);
        
        // Return the new image.
        return newImage;
    }
}

- (Boolean) CheckOutImageSize:(UIImage*)image
{
	 if (image==nil)
     {
         NSLog(@"image==nil");
         return false;
     }
    CGFloat w =image.size.width;
    CGFloat h =image.size.height;
	 CGFloat min = fmin(w, h);
	 if(min<50)
     {
         NSLog(@"min<50");
         return false;
     }
    CGFloat value = w/h;
	 if(value < (CGFloat)1/(CGFloat)3 || value>(CGFloat)60/(CGFloat)10)
     {
         NSLog(@"value < (CGFloat)1/(CGFloat)3 || value>(CGFloat)60/(CGFloat)10");
         return false;
     }
    return true;
}

+ (void) saveImageToPhotosAlbum:(NSString*) readAdr
{
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    if (status == PHAuthorizationStatusRestricted || status == PHAuthorizationStatusDenied)
    {
        NSString *errorStr = @"请在iPhone的“设置-隐私-照片”选项中，允许访问你的照片。";
        IOSCustomEmoji *app = [[IOSCustomEmoji alloc] init];
        UIViewController *viewController = UnityGetGLViewController();
        [viewController.view addSubview:app.view];
        [app showAlertMessage:errorStr];
    }
    else
    {
        [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status)
         {
             if(status == PHAuthorizationStatusAuthorized)
             {
                 NSLog(@"%@",readAdr);
                 UIImage* image = [UIImage imageWithContentsOfFile:readAdr];
                 NSLog(@"%@",image);
                 UIImageWriteToSavedPhotosAlbum(image,
                                                self,
                                                @selector(image:didFinishSavingWithError:contextInfo:),
                                                NULL);
             }
         }];
    }
}

+ (void) image:(UIImage*)image didFinishSavingWithError:(NSError*)error contextInfo:(void*)contextInfo
{
    NSString* result;
    if(error)
    {
        result = @"保存失败";
    }
    else
    {
        result = @"保存成功";
    }
    UnitySendMessage( "SDKCallBackObj", "CommonTip", result.UTF8String);
}

///是不是ipad
- (Boolean)getIsIpad
{
    NSString *deviceType = [UIDevice currentDevice].model;
    if([deviceType isEqualToString:@"iPhone"]) {
        //iPhone
        return false;
        
    }
    else if([deviceType isEqualToString:@"iPod touch"]) {
        //iPod Touch
        return false;
        
    }
    else if([deviceType isEqualToString:@"iPad"]) {
        //iPad
        return true;
    }
    return NO;
}
@end
