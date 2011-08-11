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
import javax.swing.filechooser.FileNameExtensionFilter;

public class Vijos2FPS extends JFrame implements ActionListener {
	

	private static final long SplitLimit = 8000000;
	private static String APPDIR = System.getProperty("user.dir").replaceAll(
			"\\\\", "/");

	public Vijos2FPS() {
		super("Vijos2FPS");
		String vijosDataPath = openVijosData();
		convert(vijosDataPath);
		System.exit(0);
	}

	private void convert(String vijosDataPath) {
		// TODO Auto-generated method stub
		File basedir = new File(vijosDataPath);
		try {
			String fname = vijosDataPath + "/fps.xml";
			PrintStream fps = new PrintStream(fname, "UTF-8");
			// fps=System.out;
			printHeader(fps);
			String[] items = basedir.list(new VijosFilenameFilter());
			for (int i = 0; i < items.length; i++) {
				printProblem(fps, basedir + "/" + items[i]);
				File file = new File(fname);
				if (file.length() > SplitLimit) {
					printTail(fps);
					fps.close();
					fname = vijosDataPath + "/fps-" + (i+1) + ".xml";
					fps = new PrintStream(fname, "UTF-8");
					printHeader(fps);
				}
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
		printElement(fps, "title", new File(dir).getName(), "");
		printElement(fps, "description", dir + "/Description.txt", "", true);
		printElement(fps, "input", dir + "/InputFormat.txt", "", true);
		printElement(fps, "output", dir + "/OutputFormat.txt", "", true);
		printElement(fps, "sample_input", dir + "/SampleInput.txt", "", false);
		printElement(fps, "sample_output", dir + "/SampleOutput.txt", "", false);
		printElement(fps, "time_limit", dir + "/TimeLimitation.txt", "", true);
		printElement(fps, "time_limit", "1", "unit=\"s\"");
		printElement(fps, "memory_limit", "128", "unit=\"mb\"");
		printElement(fps, "hint", dir + "/Hint.txt", "", true);
		printElement(fps, "source", dir + "/Source.txt", "", true);

		printData(fps, dir, "Input");
		printData(fps, dir, "Output");
		fps.println("</item>");

	}

	private void printData(PrintStream fps, String dir, String type) {
		File basedir = new File(dir + "/" + type);
		String[] items = basedir.list(new ExtensionFilter("txt"));
		for (int i = 0; items!=null&&i < items.length; i++) {
			printElement(fps, "test_" + type.toLowerCase(), basedir + "/"
					+ items[i], "", false);
		}
	}

	private void printElement(PrintStream fps, String tagName, String content,
			String props) {
		// TODO Auto-generated method stub
		fps.printf("<%s %s><![CDATA[", tagName, props);
		fps.printf("%s]]></%s>\n", content.replaceAll("]]>", "]]]><![CDATA[]>"), tagName);
	}

	private void printElement(PrintStream fps, String tagName, String file,
			String props, boolean pre) {
		printElement(fps, tagName, pre ? htmlEncode(readFile(file))
				: readFile(file), props);

	}

	public String htmlEncode(String s) {
		StringBuffer stringbuffer = new StringBuffer();
		int j = s.length();
		for (int i = 0; i < j; i++) {
			char c = s.charAt(i);
			switch (c) {
			case 60:
				stringbuffer.append("&lt; ");
				break;
			case 62:
				stringbuffer.append("&gt; ");
				break;
			case 38:
				stringbuffer.append("&amp; ");
				break;
			case 34:
				stringbuffer.append("&quot; ");
				break;
			case 169:
				stringbuffer.append("&copy; ");
				break;
			case 174:
				stringbuffer.append("&reg; ");
				break;
			case 165:
				stringbuffer.append("&yen; ");
				break;
			case 8364:
				stringbuffer.append("&euro; ");
				break;
			case 8482:
				stringbuffer.append("&#153; ");
				break;
			case 13:
				if (i < j - 1 && s.charAt(i + 1) == 10) {
					stringbuffer.append(" <br> ");
					i++;
				}
				break;
			case 32:
				stringbuffer.append("&nbsp ");
				break;

			default:
				stringbuffer.append(c);
				break;
			}
		}
		return new String(stringbuffer.toString());
	}

	private String readFile(String file) {
		// TODO Auto-generated method stub
		StringBuffer sb = new StringBuffer();
		try {
			BufferedReader br = new BufferedReader(new InputStreamReader(
					new FileInputStream(file), "GBK"));
			while (br.ready())
				sb.append(br.readLine() + "\n");
			br.close();

		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			// e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			// e.printStackTrace();
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
				System.exit(0);
			}

		}
		if (rVal == JFileChooser.CANCEL_OPTION) {
			System.exit(0);
		}
		return filename;
	}
}

class VijosFilenameFilter implements FilenameFilter {

	@Override
	public boolean accept(File arg0, String arg1) {
		// TODO Auto-generated method stub
		return (arg1.startsWith("P"));

	}

}
class ExtensionFilter implements FilenameFilter {
	private String ext;
	public ExtensionFilter(String ext){
		this.ext=ext;
		
	}
	@Override
	public boolean accept(File arg0, String arg1) {
		// TODO Auto-generated method stub
		 return (arg1.toLowerCase().endsWith(ext));
	}

}
