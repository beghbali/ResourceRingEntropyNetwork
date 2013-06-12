using System;
using System.Text;

namespace RRLib
{
	/// <summary>
	/// Global Constants
	/// </summary>
	public class Constants
	{
		//General 
		public static readonly string ENTROPY_PROTOCOL_VERSION = "0.1";
		public static readonly int ONESEC = 1000;
		public static readonly string[] AUDIO_FILTER = {".cda",".cdr",".mid",".mp3",".mp4",".ra",".wav"};
		//"*.669;*.aifc;*.aiff;*.ais;*.akp;*.alaw;*.ams;*ams;*.apex;*.ase;*.asf*.asx;*.au;*.avi;*.avr;*.c01;*.cda;*.cdr;*.cmf;*.dcm;*.dewf;*.df2;*.dfc;*.dig;*.dig;*.dls;*.dmf;*.dsf;*.dsm;*.dsp;*.dtm;*.dwd;*.eda;*.ede;*.edk;*.edq;*.eds;*.edv;*.efa;*.efe;*.efk;*.efq;*.efs;*.efv;*.emb;*.emd;*.esps;*.eui;*.f32;*.f64;*.f2r;*.f3r;*.far;*.fff;*.fsm;*.fzb;*.fzf;*.fzv;*.g721;*.g723;*.g726;*.gig;*.gkh;*.gsm;*.gsm;*.iff;*.ini;*.ins;*.ins;*.it;*.iti;*.its;*.k25;*.k26;*.kmp;*.krz;*.ksc;*.ksf;*.mat;*.med;*.mid;*.mod;*.mpeg;*.mp2;*.mp3;*.mp4;*.mt2;*.mte;*.mti;*.mtm;*.mtp;*.mts;*.mtx;*.mws;*.nst;*.okt;*.pac;*.pat;*.pbf;*.prg;*.phy;*.psm;*.ptm;*.ra;*.ram;*.raw;*.rbs;*.rmf;*.rol;*.rti;*.rtm;*.rts;*.s3i;*.s3m;*.sam;*.sb;*.sbk;*.sbi;*.sd;*.sd2;*.sdk;*.sds;*.sdx;*.sf;*.sf2;*.smp;*.snd;*.sou;*.sppack;*.stm;*.stx;*.sw;*.syx;*.syh;*.syw;*.td0;*.txt;*.txt;*.txw;*.ub;*.ulaw;*.ult;*.uni;*.uw;*.uwf;*.voc;*.vmd;*.vmf;*.vox;*.w01;*.wav;*.wfb;*.wfd;*.wfp;*.wow;*.xi;*.xm;*.xp;*.xt";

		public static readonly string[] VIDEO_FILTER = {".wm",".asf",".wmv",".avi",".mpg",".mpeg",".mpe",".m1v",".mp2",".mpv2",".mov",".rm"};

		public static readonly string[] DOCUMENT_FILTER = {".txt",".doc",".xsl",".ppt",".pdf",".ps"};

		public static readonly string[] GRAPHICS_FILTER = {".jpg",".jpeg",".bmp",".gif",".pic",".png",".tiff"};

		public static readonly string[] BINARY_FILTER = {".exe",".zip",".Z",".gz"};

		//NOTE: order in this array must _NOT_ be changed, add at the end, modify the ResourceType
		//enum in a matching order as well
		public static readonly string[][] RESOURCE_FILTERS = {AUDIO_FILTER, VIDEO_FILTER, DOCUMENT_FILTER, GRAPHICS_FILTER, BINARY_FILTER};
		public static readonly char[] TOKENIZER_DELIMETERS = {' ','\n','\r','\t','.','-',',',';','"','\'','+','-',')','(','*','&','^','%','$','#','@','@','!','~','`','[',']','{','}','|','\\','/','?','>','<','_'};
		public static readonly string[] ASCIIDOCS_EXTENSIONS = {".txt"};

		public static readonly string[] STOPWORDS = {"to","from","on","into","the","onto","upon","that","this"};
		public static readonly uint DEFAULT_TOKENS = 1;
		
		public static readonly long KILOBYTE = 1024;
		public static readonly long MEGABYTE = 1024 * KILOBYTE;
		public static readonly long GIGABYTE = 1024 * MEGABYTE;
		public static readonly long TERABYTE = 1024 * GIGABYTE;

		public static readonly int IE_MAX_KEYWORDS_TO_CONSIDER = 15;
		public static readonly int MAX_PEERS_TO_CONSIDER_FOR_KEYWORD = 3;
		public static readonly int MAX_PEERS = 5;
		public static readonly int MAX_PEERS_TO_QUERY = 5;   //If neighbor choice algorithm fails, pick at most this many
		public static readonly int MAX_QUERY_MATCHES = 100;
		public static readonly int MAX_HOPS = 20;
		public static readonly int MAX_RESOURCES_TO_CONSIDER_FOR_KEYWORD = 15;
		public static readonly int MAX_SEARCH_RESULTS_FROM_PEER = 200; 
		public static readonly float PEER_SELECTION_THRESHOLD = 0.4f;
		public static readonly float QUERY_MATCH_THRESHOLD = 0.7f;
		
		public static readonly string[] NULL_STRING_ARRAY = {};
		
		public static readonly IPEndPoint ANY_PORT = new IPEndPoint(System.Net.IPAddress.Any, 0);
		public static readonly int MAX_ROUTER_THREADS = 10;
		public static readonly int MAX_SERVER_THREADS = 500;
		
		public static readonly int MAX_INFORMATION_ENTROPY_LENGTH = 50;

		public static readonly ulong MAX_RESOURCE_ID = Int64.MaxValue;

		public static readonly int MAX_SIMULTANEOUS_DOWNLOADS = 6;

		public static readonly int MAX_DOWNLOAD_PARTITION = 5;

		//Server properties
		public static readonly string SERVER_IPADDRESS_STRING = "192.168.1.104";
		public static readonly string SERVER_IPADDRESS_STRING2 = "192.168.1.104";
		public static readonly System.Net.IPAddress SERVER_IPADDRESS = System.Net.IPAddress.Parse(SERVER_IPADDRESS_STRING);
		public static readonly System.Net.IPAddress SERVER_IPADDRESS2 = System.Net.IPAddress.Parse(SERVER_IPADDRESS_STRING2);
		public static readonly int SERVER_PORT = 4225;
		public static readonly IPEndPoint SERVER = new IPEndPoint(Constants.SERVER_IPADDRESS, Constants.SERVER_PORT);
		public static readonly IPEndPoint SERVER2 = new IPEndPoint(Constants.SERVER_IPADDRESS2, Constants.SERVER_PORT);
		
		public static readonly int MAX_CONNECTED_CLIENTS = 100;

		public static readonly int QUERYCACHE_INVALIDATE_PERIOD = 1200000;  //This is how often the background process runs

		//Peer properties
		public static readonly uint NUMADJPEERS = 3;
		public static readonly int NUMINITIALPEERS = 1000;
		public static readonly int MIN_NUM_NEIGHBORS = 1;	
		//System properties

		//NOTE: the following two need to be the same because the client assumes that messages will
		//not be longer than these values so the server should not send anything longer.
		public static readonly uint READBUFFSIZE = 8192;
		public static readonly uint WRITEBUFFSIZE = 8192;

		//Form properties
		public static readonly uint MAXZIPCODE = 99999;

		//Client properties
		public static readonly int LOGINRETRYINTERVAL = 3*ONESEC;
		public static readonly uint MAXLOGINRETRIES = 5;
		public static readonly int WAITFORLOGININTERVAL = 8*ONESEC;
		public static readonly int CLIENTSYNCLISTENPORT = 4657;
		public static readonly int CLIENTASYNCLISTENPORT = 5657;
		public static readonly int CLIENTLISTENPORRANGE = 8000;
		public static readonly Node BASHIR_PC = new Node(new IPEndPoint(SERVER_IPADDRESS, CLIENTSYNCLISTENPORT), 
			new IPEndPoint(SERVER_IPADDRESS, CLIENTASYNCLISTENPORT), Constants.LineSpeedType.LNSPEED_DSL);
		public static readonly Node JANA_PC = new Node(new IPEndPoint(SERVER_IPADDRESS, CLIENTSYNCLISTENPORT), 
			new IPEndPoint(SERVER_IPADDRESS, CLIENTASYNCLISTENPORT), Constants.LineSpeedType.LNSPEED_DSL);

		//Session properties
		public static readonly ulong INVALIDSESSIONID = 0;
	
		public static readonly int MAXTCPRECEIVEWAIT = 3000;
		public static readonly int MAXTCPSENDWAIT = 3000;

		//Ring properties
		public static readonly uint NUM_RINGS = 5; //If you add a ring update this variable
		public static readonly string AUDIO_RING_NAME = "Audio Ring";
		public static readonly string VIDEO_RING_NAME = "Video Ring";
		public static readonly string GRAPHICS_RING_NAME = "Graphics Ring";
		public static readonly string DOCUMENTS_RING_NAME = "Documents Ring";
		public static readonly string BINARY_RING_NAME = "Binary Ring";
		public static readonly uint AUDIO_RING_ID = 1;
		public static readonly uint VIDEO_RING_ID = 2;
		public static readonly uint GRAPHICS_RING_ID = 3;
		public static readonly uint DOCUMENTS_RING_ID = 4;
		public static readonly uint BINARY_RING_ID = 5;
		public static readonly IPEndPoint AUDIO_LORD_NODE = SERVER;
		public static readonly IPEndPoint VIDEO_LORD_NODE = SERVER;
		public static readonly IPEndPoint GRAPHICS_LORD_NODE = SERVER;
		public static readonly IPEndPoint DOCUMENTS_LORD_NODE = SERVER;
		public static readonly IPEndPoint BINARY_LORD_NODE = SERVER;
		public static readonly LordInfo AUDIO_LORD = new LordInfo(AUDIO_RING_ID, AUDIO_LORD_NODE);
		public static readonly LordInfo VIDEO_LORD = new LordInfo(VIDEO_RING_ID, VIDEO_LORD_NODE);
		public static readonly LordInfo GRAPHICS_LORD = new LordInfo(GRAPHICS_RING_ID, GRAPHICS_LORD_NODE);
		public static readonly LordInfo DOCUMENTS_LORD = new LordInfo(DOCUMENTS_RING_ID, DOCUMENTS_LORD_NODE);
		public static readonly LordInfo BINARY_LORD = new LordInfo(BINARY_RING_ID, BINARY_LORD_NODE);
		public static readonly LordInfo[] DEFAULT_LORDS = {AUDIO_LORD, VIDEO_LORD, GRAPHICS_LORD, DOCUMENTS_LORD, BINARY_LORD};
		
		public static readonly FileGroup[] AUDIO_GROUP = {new FileGroup("Audio", Constants.AUDIO_FILTER)};
		public static readonly FileGroup[] VIDEO_GROUP = {new FileGroup("Video", Constants.VIDEO_FILTER)};
		public static readonly FileGroup[] DOCUMENTS_GROUP = {new FileGroup("Documents", Constants.DOCUMENT_FILTER)};
		public static readonly FileGroup[] GRAPHICS_GROUP = {new FileGroup("Graphics", Constants.GRAPHICS_FILTER)};
		public static readonly FileGroup[] BINARY_GROUP = {new FileGroup("Binary", Constants.BINARY_FILTER)};
			
		public static readonly Ring AUDIO_RING = new Ring(AUDIO_RING_ID, AUDIO_RING_NAME, AUDIO_FILTER, AUDIO_GROUP);
		public static readonly Ring VIDEO_RING = new Ring(VIDEO_RING_ID, VIDEO_RING_NAME, VIDEO_FILTER, VIDEO_GROUP);
		public static readonly Ring GRAPHICS_RING = new Ring(GRAPHICS_RING_ID, GRAPHICS_RING_NAME, GRAPHICS_FILTER, GRAPHICS_GROUP);
		public static readonly Ring DOCUMENTS_RING = new Ring(DOCUMENTS_RING_ID, DOCUMENTS_RING_NAME, DOCUMENT_FILTER, DOCUMENTS_GROUP);
		public static readonly Ring BINARY_RING = new Ring(BINARY_RING_ID, BINARY_RING_NAME, BINARY_FILTER, BINARY_GROUP);
		public static readonly Ring[] DEFAULT_RINGS = {AUDIO_RING, VIDEO_RING, GRAPHICS_RING, DOCUMENTS_RING, BINARY_RING};
		public static readonly RingInfo AUDIO_RING_INFO = new RingInfo(AUDIO_RING, "","");
		public static readonly RingInfo VIDEO_RING_INFO = new RingInfo(VIDEO_RING, "","");
		public static readonly RingInfo GRAPHICS_RING_INFO = new RingInfo(GRAPHICS_RING, "","");
		public static readonly RingInfo DOCUMENTS_RING_INFO = new RingInfo(DOCUMENTS_RING, "","");
		public static readonly RingInfo BINARY_RING_INFO = new RingInfo(BINARY_RING, "","");
		public static readonly RingInfo[] DEFAULT_RINGSINFO = {AUDIO_RING_INFO, VIDEO_RING_INFO, GRAPHICS_RING_INFO, DOCUMENTS_RING_INFO, BINARY_RING_INFO};

		//User properties
		public static readonly Member BASHIR = new Member("beghbali", ASCIIEncoding.ASCII.GetBytes("1234"), new PublicUserInfo("green thpike"), new PrivateUserInfo(1000, "Mr.", "Bashir", "", "Eghbali", "", "4132 Valerie Dr.", "Campbell", 95008, "CA", "USA"), BASHIR_PC, DEFAULT_LORDS); 
			
		public static readonly Member JANA = new Member("janakov", ASCIIEncoding.ASCII.GetBytes("4321"), new PublicUserInfo("orange thpike"), new PrivateUserInfo(1001, "Ms.", "Jana", "", "Kovacevic", "", "4132 Valerie Dr.", "Campbell", 95008, "CA", "USA"), JANA_PC, DEFAULT_LORDS); 
			
		public static readonly uint INVALIDUSERID = 0;

			
		[Serializable]
		public enum MessageTypes : uint
		{
			MSG_UNKNOWNERROR,
			MSG_LOGIN,
			MSG_LOGOFF,
			MSG_SIGNUP,
			MSG_SYNCUSRINFO,
			MSG_OK, 
			MSG_NOTAMEMBER,
			MSG_ALREADYSIGNEDIN,
			MSG_RINGLOGIN, 
			MSG_RINGNEIGHBORS, 
			MSG_HELLO, 
			MSG_DISCONNECT,
			MSG_SIMPLEQUERY,
			MSG_COLUMNQUERY,
			MSG_FORMQUERY,
			MSG_QUERYFORMED, 
			MSG_QUERYHIT, 
			MSG_DOWNLOADREQUEST, 
			MSG_DOWNLOADREPLY, 
			MSG_RESOURCENOTFOUND
		}

		public enum LoginStatus : uint
		{
			STATUS_LOGGEDIN,
			STATUS_SERVERNOTREACHED,
			STATUS_NOTAMEMBER, 
			STATUS_ALREADYSIGNEDIN
		}

		public enum ResourceType : uint
		{
			UNKNOWN_RESOURCE_TYPE = 0,
			FILE_TYPE_AUDIO       = 1,
			FILE_TYPE_VIDEO       = 2,
			FILE_TYPE_DOC         = 3,
			FILE_TYPE_GRAPHICS    = 4
		}

		public enum QueryHandlerType : uint
		{
			SERVER_FORM_QUERY,      //server takes search string and returns appropriate query string
			SERVER_RESOLVE_QUERY,   //server performs the query search and returns results
			DISTRIBUTED_PEER_QUERY  //client does distributed peer to peer search via Entropy protocol
		}

		public enum LineSpeedType : uint
		{
			LNSPEED_UNKNOWN,
			LNSPEED_MODEM_14K = 14000,
			LNSPEED_MODEM_28K = 28000,
			LNSPEED_MODEM_56K = 56000,
			LNSPEED_MODEM_128K = 128000,
			LNSPEED_DSL,
			LNSPEED_CABLE,
			LNSPEED_T1,
			LNSPEED_OC1
		}
}
}
