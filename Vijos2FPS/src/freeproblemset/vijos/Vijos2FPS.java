package freeproblemset.vijos;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.FilenameFilter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintStream;

import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.filechooser.FileFilter;

public class Vijos2FPS  extends JFrame implements ActionListener{
	private static String APPDIR = System.getProperty("user.dir").replaceAll(
			"\\\\", "/");
	public Vijos2FPS(){
		super("Vijos2FPS");
		String vijosDataPath = openVijosData();
		convert(vijosDataPath);
		System.exit(0);
	}
	private void convert(String vijosDataPath) {
		// TODO Auto-generated method stub
		File basedir=new File(vijosDataPath);
		try {
			PrintStream fps=new PrintStream(vijosDataPath+"/fps.xml","UTF-8");
			//fps=System.out;
			printHeader(fps);
			String [] items = basedir.list( new VijosFilenameFilter());
			for(int i=0;i<items.length;i++){
				printProblem(fps,basedir+"/"+items[i]);
			}
			printTail(fps);
			fps.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	private void printTail(PrintStream fps) {
		// TODO Auto-generated method stub
		fps.println("</fps>");
	}
	private void printProblem(PrintStream fps, String dir) {
		// TODO Auto-generated method stub
		fps.println("<item>");
		printElement(fps,"title",new File(dir).getName(),"");
		printElement(fps,"description",dir+"/Description.txt","",true);
		printElement(fps,"input",dir+"/InputFormat.txt","",true);
		printElement(fps,"output",dir+"/OutputFormat.txt","",true);
		printElement(fps,"sample_input",dir+"/SampleInput.txt","",false);
		printElement(fps,"sample_output",dir+"/SampleOutput.txt","",false);
		printElement(fps,"time_limit",dir+"/TimeLimitation.txt","",true);
		printElement(fps,"time_limit","1","unit=\"s\"");
		printElement(fps,"memory_limit","128","unit=\"mb\"");
		printElement(fps,"hint",dir+"/Hint.txt","",true);
		printElement(fps,"source",dir+"/Source.txt","",true);
		
		printData(fps, dir,"Input");
		printData(fps, dir,"Output");
		fps.println("</item>");
		
		
	}
	private void printData(PrintStream fps, String dir,String type) {
		File basedir=new File(dir+"/"+type);
		String [] items = basedir.list();
		for(int i=0;i<items.length;i++){
			printElement(fps,"test_"+type.toLowerCase(),basedir+"/"+items[i],"",false);
		}
	}
	private void printElement(PrintStream fps,String tagName, String content, String props) {
		// TODO Auto-generated method stub
		fps.printf("<%s %s><![CDATA[",tagName,props);
		fps.printf("%s]]></%s>\n",content,tagName);
	}
	private void printElement(PrintStream fps, String tagName, String file,
			String props,boolean pre) {
		// TODO Auto-generated method stub
		fps.printf("<%s %s><![CDATA["+(pre?"<pre>":""),tagName,props);
		String content=readFile(file);
		fps.printf("%s"+(pre?"</pre>":"")+"]]></%s>\n",content,tagName);
		
	}
	private String readFile(String file) {
		// TODO Auto-generated method stub
		StringBuffer sb=new StringBuffer();
		try {
			BufferedReader br=new BufferedReader(new InputStreamReader(new FileInputStream(file),"GBK"));
			while(br.ready())
				sb.append(br.readLine()+"\n");
			br.close();
		
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
		}
		
		return sb.toString();
	}
	private void printHeader(PrintStream fps) {
		// TODO Auto-generated method stub
		fps.print("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n <fps version=\"1.2\">\n");
	}
	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		new Vijos2FPS();
	}

	@Override
	public void actionPerformed(ActionEvent arg0) {
		// TODO Auto-generated method stub
		
	}
	public String openVijosData() {
		String filename = null;
		JFileChooser c = new JFileChooser(APPDIR);
		c.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
		// Demonstrate "Open" dialog:
		int rVal = c.showOpenDialog(this);
		if (rVal == JFileChooser.APPROVE_OPTION) {
			try {
				filename = (c.getSelectedFile().getCanonicalPath());
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
				filename = "jar:file:" + APPDIR + "/FPSViewer.jar!/demo.xml";
			}

		}
		if (rVal == JFileChooser.CANCEL_OPTION) {
			filename = "jar:file:" + APPDIR + "/FPSViewer.jar!/demo.xml";

		}
		return filename;
	}
}
class VijosFilenameFilter  implements FilenameFilter{

	@Override
	public boolean accept(File arg0, String arg1) {
		// TODO Auto-generated method stub
		return (arg1.startsWith("P"));
		
	}
	
}
