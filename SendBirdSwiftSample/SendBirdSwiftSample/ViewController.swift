//
//  ViewController.swift
//  SendBirdSwiftSample
//
//  Created by Jed Kyung on 1/30/16.
//  Copyright © 2016 SENDBIRD.COM. All rights reserved.
//

import UIKit
import SendBirdSDK

class ViewController: UIViewController, UITextFieldDelegate {

    var sendbirdLogoImageView: UIImageView?
    var sendbirdLabel: UILabel?
    var backgroundImageView: UIImageView?
    var sendbirdStartOpenChatButton: UIButton?
    var sendbirdStartMessaging: UIButton?
    var sendbirdMemberListButton: UIButton?
    var sendbirdMessagingChannelList: UIButton?
    var sendbirdBackFromMessaging: UIButton?
    var sendbirdChannelListButton: UIButton?
    var sendbirdLobbyMemberListButton: UIButton?
    var sendbirdMessagingChannelListButton: UIButton?
    var sendbirdUserNicknameLabel: UILabel?
    var sendbirdUserNicknameTextField: UITextField?
    
    private var messagingUserName: NSString?
    private var messagingUserId: NSString?
    private var messagingTargetUserId: NSString?
    private var startMessagingFromOpenChat: Bool?

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        
        initView()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    func initView() {
        navigationController?.setNavigationBarHidden(true, animated: false)
        
        backgroundImageView = UIImageView(image: UIImage(named: "_sendbird_img_bg_default.jpg"))
        backgroundImageView?.translatesAutoresizingMaskIntoConstraints = false
        backgroundImageView?.contentMode = UIViewContentMode.ScaleAspectFill
        backgroundImageView?.clipsToBounds = true
        view.addSubview(backgroundImageView!)
        
        // SendBird Logo
        sendbirdLogoImageView = UIImageView(image: UIImage(named: "_logo"))
        sendbirdLogoImageView?.translatesAutoresizingMaskIntoConstraints = false
        view.addSubview(sendbirdLogoImageView!)

        NSLog("Version: %@", SendBird.VERSION())
        sendbirdLabel = UILabel()
        sendbirdLabel?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdLabel?.text = NSString.init(format: "SendBird v%@", SendBird.VERSION()) as String
        sendbirdLabel?.textColor = UIColor.whiteColor()
        sendbirdLabel?.font = UIFont.init(name: "AmericanTypewriter-Bold", size: 28.0)
        sendbirdLabel?.hidden = true
        view.addSubview(sendbirdLabel!)
        
        // SendBird User Nickname Label
        sendbirdUserNicknameLabel = UILabel()
        sendbirdUserNicknameLabel?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdUserNicknameLabel?.text = "Enter your nickname."
        sendbirdUserNicknameLabel?.textColor = UIColor.whiteColor()
        sendbirdUserNicknameLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        view.addSubview(sendbirdUserNicknameLabel!)
        
        // SendBird User Nickname
        sendbirdUserNicknameTextField = UITextField()
        sendbirdUserNicknameTextField?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdUserNicknameTextField?.background = SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0xE8EAF6))
        sendbirdUserNicknameTextField?.clipsToBounds = true
        sendbirdUserNicknameTextField?.layer.cornerRadius = 4.0
        var leftPaddingView: UIView?
        var rightPaddingView: UIView?
        leftPaddingView = UIView.init(frame: CGRectMake(0, 0, 12, 0))
        rightPaddingView = UIView.init(frame: CGRectMake(0, 0, 12, 0))
        sendbirdUserNicknameTextField?.leftView = leftPaddingView
        sendbirdUserNicknameTextField?.leftViewMode = UITextFieldViewMode.Always
        sendbirdUserNicknameTextField?.rightView = rightPaddingView
        sendbirdUserNicknameTextField?.rightViewMode = UITextFieldViewMode.Always
        sendbirdUserNicknameTextField?.placeholder = "Nickname"
        sendbirdUserNicknameTextField?.font = UIFont.systemFontOfSize(16.0)
        sendbirdUserNicknameTextField?.returnKeyType = UIReturnKeyType.Done
        sendbirdUserNicknameTextField?.delegate = self
        
        // Set Default User Nickname
        var USER_ID: NSString?
        var USER_NAME: NSString?
        
        USER_ID = SendBird.deviceUniqueID()
        USER_NAME = NSString.init(format: "User-%@", (USER_ID?.substringToIndex(5))!)
        sendbirdUserNicknameTextField?.text = USER_NAME as? String
        view.addSubview(sendbirdUserNicknameTextField!)
        
        // Start Open Chat Button
        sendbirdStartOpenChatButton = UIButton()
        sendbirdStartOpenChatButton?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdStartOpenChatButton?.setBackgroundImage(SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0xAB47BC)), forState: UIControlState.Normal)
        sendbirdStartOpenChatButton?.clipsToBounds = true
        sendbirdStartOpenChatButton?.layer.cornerRadius = 4.0
        sendbirdStartOpenChatButton?.addTarget(self, action:"clickSendBirdStartOpenChatButton:", forControlEvents: UIControlEvents.TouchUpInside)
        sendbirdStartOpenChatButton?.setTitle("OpenChat", forState: UIControlState.Normal)
        sendbirdStartOpenChatButton?.setTitleColor(UIColor.whiteColor(), forState: UIControlState.Normal)
        sendbirdStartOpenChatButton?.titleLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        view.addSubview(sendbirdStartOpenChatButton!)

        // Start Messaging Button
        sendbirdStartMessaging = UIButton()
        sendbirdStartMessaging?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdStartMessaging?.setBackgroundImage(SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0xAB47BC)), forState: UIControlState.Normal)
        sendbirdStartMessaging?.clipsToBounds = true
        sendbirdStartMessaging?.layer.cornerRadius = 4.0
        sendbirdStartMessaging?.addTarget(self, action:"clickSendBirdStartMessagingButton:", forControlEvents: UIControlEvents.TouchUpInside)
        sendbirdStartMessaging?.setTitle("Messaging", forState: UIControlState.Normal)
        sendbirdStartMessaging?.setTitleColor(UIColor.whiteColor(), forState: UIControlState.Normal)
        sendbirdStartMessaging?.titleLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        view.addSubview(sendbirdStartMessaging!)

        // Member List Button
        sendbirdMemberListButton = UIButton()
        sendbirdMemberListButton?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdMemberListButton?.setBackgroundImage(SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0xAB47BC)), forState: UIControlState.Normal)
        sendbirdMemberListButton?.clipsToBounds = true
        sendbirdMemberListButton?.layer.cornerRadius = 4.0
        sendbirdMemberListButton?.addTarget(self, action:"clickSendBirdMemberListButton:", forControlEvents: UIControlEvents.TouchUpInside)
        sendbirdMemberListButton?.setTitle("Member List", forState: UIControlState.Normal)
        sendbirdMemberListButton?.setTitleColor(UIColor.whiteColor(), forState: UIControlState.Normal)
        sendbirdMemberListButton?.titleLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        sendbirdMemberListButton?.hidden = true
        view.addSubview(sendbirdMemberListButton!)
        
        // Messaging Channel List Button
        sendbirdMessagingChannelListButton = UIButton()
        sendbirdMessagingChannelListButton?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdMessagingChannelListButton?.setBackgroundImage(SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0xAB47BC)), forState: UIControlState.Normal)
        sendbirdMessagingChannelListButton?.clipsToBounds = true
        sendbirdMessagingChannelListButton?.layer.cornerRadius = 4.0
        sendbirdMessagingChannelListButton?.addTarget(self, action:"clickSendBirdMessagingChannelListButton:", forControlEvents: UIControlEvents.TouchUpInside)
        sendbirdMessagingChannelListButton?.setTitle("Messaging Channel List", forState: UIControlState.Normal)
        sendbirdMessagingChannelListButton?.setTitleColor(UIColor.whiteColor(), forState: UIControlState.Normal)
        sendbirdMessagingChannelListButton?.titleLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        sendbirdMessagingChannelListButton?.hidden = true
        view.addSubview(sendbirdMessagingChannelListButton!)
        
        // Back From Messaging Button
        sendbirdBackFromMessaging = UIButton()
        sendbirdBackFromMessaging?.translatesAutoresizingMaskIntoConstraints = false
        sendbirdBackFromMessaging?.setBackgroundImage(SendBirdUtils.imageFromColor(SendBirdUtils.UIColorFromRGB(0x43A047)), forState: UIControlState.Normal)
        sendbirdBackFromMessaging?.clipsToBounds = true
        sendbirdBackFromMessaging?.layer.cornerRadius = 4.0
        sendbirdBackFromMessaging?.addTarget(self, action:"clickSendBirdBackFromMessaging:", forControlEvents: UIControlEvents.TouchUpInside)
        sendbirdBackFromMessaging?.setTitle("Back", forState: UIControlState.Normal)
        sendbirdBackFromMessaging?.setTitleColor(UIColor.whiteColor(), forState: UIControlState.Normal)
        sendbirdBackFromMessaging?.titleLabel?.font = UIFont.boldSystemFontOfSize(16.0)
        sendbirdBackFromMessaging?.hidden = true
        view.addSubview(sendbirdBackFromMessaging!)
        
        setConstraints()
    }
    
    func setConstraints() {
        // Background Image
        view.addConstraint(NSLayoutConstraint.init(item: backgroundImageView!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.Top, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: backgroundImageView!, attribute: NSLayoutAttribute.Bottom, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: backgroundImageView!, attribute: NSLayoutAttribute.Leading, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.Leading, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: backgroundImageView!, attribute: NSLayoutAttribute.Trailing, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.Trailing, multiplier: 1, constant: 0))
        
        // SendBird Logo
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLogoImageView!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLogoImageView!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.Top, multiplier: 1, constant: 48))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLogoImageView!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 80))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLogoImageView!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 76.4))
        
        // SendBird Label
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLabel!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdLogoImageView, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 8))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdLabel!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdLogoImageView, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        
        // SendBird User Nickname Label
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameLabel!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameLabel!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdLabel, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 20))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameLabel!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameLabel!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))
        
        // SendBird User Nickname TextField
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameTextField!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameTextField!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdUserNicknameLabel, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 4))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameTextField!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdUserNicknameTextField!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))
        
        // SendBird Start Open Chat Button
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartOpenChatButton!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartOpenChatButton!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdUserNicknameTextField, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 40))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartOpenChatButton!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartOpenChatButton!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))
        
        // SendBird Start Messaging Button.
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartMessaging!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartMessaging!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdStartOpenChatButton, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 12))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartMessaging!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdStartMessaging!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))
        
        // SendBird Member List Button
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMemberListButton!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMemberListButton!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdUserNicknameTextField, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 40))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMemberListButton!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMemberListButton!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))

        // SendBird Messaging Channel List.
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMessagingChannelListButton!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMessagingChannelListButton!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdMemberListButton, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 12))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMessagingChannelListButton!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdMessagingChannelListButton!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))

        // Back From Messaging Button
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdBackFromMessaging!, attribute: NSLayoutAttribute.CenterX, relatedBy: NSLayoutRelation.Equal, toItem: view, attribute: NSLayoutAttribute.CenterX, multiplier: 1, constant: 0))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdBackFromMessaging!, attribute: NSLayoutAttribute.Top, relatedBy: NSLayoutRelation.Equal, toItem: sendbirdMessagingChannelListButton, attribute: NSLayoutAttribute.Bottom, multiplier: 1, constant: 12))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdBackFromMessaging!, attribute: NSLayoutAttribute.Width, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 220))
        view.addConstraint(NSLayoutConstraint.init(item: sendbirdBackFromMessaging!, attribute: NSLayoutAttribute.Height, relatedBy: NSLayoutRelation.Equal, toItem: nil, attribute: NSLayoutAttribute.NotAnAttribute, multiplier: 1, constant: 36))
    }

    override func viewWillAppear(animated: Bool) {
        super.viewWillAppear(animated)
        UIApplication.sharedApplication().statusBarStyle = UIStatusBarStyle.LightContent
        NSNotificationCenter.defaultCenter().addObserver(self, selector: "startMessagingWithUser:", name: "open_messaging", object: nil)
    }
    
    override func viewDidAppear(animated: Bool) {
        super.viewDidAppear(animated)
        
        if startMessagingFromOpenChat == true {
            /*
            MessagingTableViewController *viewController = [[MessagingTableViewController alloc] init];
            
            [viewController setViewMode:kMessagingViewMode];
            [viewController initChannelTitle];
            [viewController setChannelUrl:@""];
            [viewController setUserName:messagingUserName];
            [viewController setUserId:messagingUserId];
            [viewController setTargetUserId:messagingTargetUserId];
            
            UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:viewController];
            [self presentViewController:navigationController animated:YES completion: nil];
            */
        }
        
        startMessagingFromOpenChat = false
    }
    
    private func startMessagingWithUser(obj: NSNotification) {
        self.messagingTargetUserId = obj.object as! String
        startMessagingFromOpenChat = true
    }
    
    func clickSendBirdStartOpenChatButton(sender: AnyObject) {
        NSLog("clickSendBirdStartOpenChatButton")
        if self.sendbirdUserNicknameTextField?.text?.characters.count > 0 {
            self.startSendBird((self.sendbirdUserNicknameTextField?.text)!, chatMode: kChatModeChatting, viewMode: kChannelListViewMode)
        }
    }
    
    func clickSendBirdStartMessagingButton(sender: AnyObject) {
        NSLog("clickSendBirdStartMessagingButton")
        sendbirdStartOpenChatButton?.hidden = true
        sendbirdStartMessaging?.hidden = true
        sendbirdMemberListButton?.hidden = false
        sendbirdMessagingChannelListButton?.hidden = false
        sendbirdBackFromMessaging?.hidden = false
    }
    
    func clickSendBirdMemberListButton(sender: AnyObject) {
        NSLog("clickSendBirdMemberListButton")
        if self.sendbirdUserNicknameTextField?.text?.characters.count > 0 {
//           [self startSendBirdWithUserName:[self.sendbirdUserNicknameTextField text] andChatMode:kChatModeMessaging andViewMode:(int)kMessagingMemberViewMode];
        }
    }
    
    func clickSendBirdMessagingChannelListButton(sender: AnyObject) {
        NSLog("clickSendBirdMessagingChannelListButton")
    }
    
    func clickSendBirdBackFromMessaging(sender: AnyObject) {
        NSLog("clickSendBirdBackFromMessaging")
        sendbirdStartOpenChatButton?.hidden = false
        sendbirdStartMessaging?.hidden = false
        sendbirdMemberListButton?.hidden = true
        sendbirdMessagingChannelListButton?.hidden = true
        sendbirdBackFromMessaging?.hidden = true
    }
    
    private func startSendBird(userName: String, chatMode: Int, viewMode: Int) {
        let APP_ID: String = "A7A2672C-AD11-11E4-8DAA-0A18B21C2D82"
        let USER_ID: String = SendBird.deviceUniqueID()
        let USER_NAME: String = userName
        
        self.messagingUserName = USER_NAME
        self.messagingUserId = USER_ID
        
        if chatMode == kChatModeChatting {
            let viewController: ChattingTableViewController = ChattingTableViewController()
            SendBird.initAppId(APP_ID, withDeviceId: SendBird.deviceUniqueID())
            
            viewController.setViewMode(viewMode)
            viewController.initChannelTitle()
            viewController.userName = USER_NAME
            viewController.userId = USER_ID
            
            self.navigationController?.pushViewController(viewController, animated: false)
        }
        else {
            /*
            MessagingTableViewController *viewController = [[MessagingTableViewController alloc] init];
            
            [SendBird initAppId:APP_ID withDeviceId:[SendBird deviceUniqueID]];
            
            [viewController setViewMode:viewMode];
            [viewController initChannelTitle];
            [viewController setChannelUrl:CHANNEL_URL];
            [viewController setUserName:USER_NAME];
            [viewController setUserId:USER_ID];
            
            [self.navigationController pushViewController:viewController animated:NO];
            */
        }
    }
    
    // MARK: UITextFieldDelegate
    func textFieldShouldReturn(textField: UITextField) -> Bool {
        textField.resignFirstResponder()
        return true
    }
}

