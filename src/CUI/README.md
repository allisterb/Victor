# Victor CX
Victor CX is a 100% open source client-server system which provides a multi-modal conversational user interface client and server back-end for interacting with an organisation's technology products and online services like product management and administration, product documentation, customer service and support, business processes like applying for a loan or for admission to a school, and other customer experience applications that traditionally rely on a user's ability to navigate a complex GUI and on the visual presentation of documents and forms. 

Victor CX is specifically designed for users who are blind or sight-impaired or who are otherwise not able to effectively use a traditional GUI with mouse or touchscreen input, and must rely on assistive technologies like screen-readers or braille displays. The client uses a simplified, conversation-driven interface powered by natural language understanding that can run either on a character-based terminal or as a browser as a web application. Victor produces line-by-line output that is easily read by screen readers and line-driven interactive input that can be easily entered via any kind of keyboard or character input device, or via speech recognition for users who cannot use such devices.

The back-end consists of a scalable chatbot server which contains the CUI logic for different bots and tracks and persists the conversation state for each client interaction with a bot, as well as as a web API which is called by CUI bots to retrieve content or run operations or interact with an organization's existing services.

Victor CX lets organizations create auditory user interfaces for customer experience that integrate with existing business processes and services and content and which also satisfy the 7 [inclusive design principles](https://inclusivedesignprinciples.org/)

* Provide comparable experience

* Consider situation

* Be consistent

* Give control

* Offer choice

* Prioritise content

* Add value

Victor CX provides a 100% open-source alternative to proprietary CUI services like Google's Dialogflow, Microsoft's LUIS, IBM's Watson, Amazon's Alexa, wit.al, et.al. Unlike existing open-source chatbot projects like [Rasa](https://rasa.com/), Victor CX is designed around microservices and RedHat's OpenShift Container Platform and open-source enterprise-grade servers and frameworks like MongoDB, .NET Core, and Java EE, and can be scaled to reliably handle millions of conversations. The client program is designed to work efficiently with screen-readers and other assistive technology and can be run on computers without any GUI environments installed like traditional *nix and BSD systems. 

Non-visual software developers and administrators can use a Victor CX feature called the Voice Interactive Shell (Vish) which provides a CUI using natural language understanding for system administration that offers an alternative to using complex heavy Web GUIs or command-line tools that require precise syntax and extensive memorization of options. Using Vish users can express intents in natural language (like `show me all  running pods`) without having to memorize an exact command syntax and options and the CUI will format and break up the command output into manageable pieces that can be easily read via a screen reader and navigate through the output using the keyboard.

## Implementation
Victor CX implements CUIs as hierarchical [packages](https://github.com/allisterb/Victor/blob/master/src/Base/Victor.Base.CUI/CUIPackage.cs) which group small sets of [related tasks](https://github.com/allisterb/Victor/blob/master/src/CUI/Victor.CUI.Vish/OpenShift/OpenShift.cs).

There are 3 package categories:
* Vish - Visual Interactive Shell packages for non-visual users performing system administration tasks like managing an OpenShift cluster
* Services - Tasks that do not require much interactivity like checking product news
* Bots - Bots are conversational agents that help you with tasks like filling out complex forms or completing complex multi-step processes and workflows that require a lot of interactivity.

Victor CX [controllers](https://github.com/allisterb/Victor/blob/master/src/Interfaces/Victor.CLI/CXController.cs) have the task of interfacing between CUI packages and the user's display and input devices and NLU and ASR components. The only Controller implemented right now is the [CLI](https://github.com/allisterb/Victor/blob/master/src/Interfaces/Victor.CLI/CXController.cs)

