#include <stdio.h>
#include <string.h>
int main(int argc,char * argv[]){

const char * head1="<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
const char * head2="<fps version=\"1.2\" url=\"https://github.com/zhblue/freeproblemset/\">";
const char * head3="<generator name=\"HUSTOJ\" url=\"https://github.com/zhblue/hustoj/\"/>";
const char * tail="</item>\n</fps>\n";
		if(argc<2){
			printf("Usage: fpssliter <fpsfile.xml> [size]\n \t http://github.com/zhblue/freeproblemset\n");
			return -1;
		}else{
			printf("Splitting freeproblemset file: %s\n",argv[1]);
		 }
		FILE * infile=fopen(argv[1],"r");
		int num=1;
		const int bsize=64000;
		int trigger=10;
		if (argc==3){
			sscanf(argv[2],"%d",&trigger);
		}
		
		int location=0;
		char filename[255] ;
		char buf [bsize] ;
		char * endpoint;
		int cp=0;
		sprintf(filename,"%s_%d.xml",argv[1],num);
		FILE * outfile=fopen(filename,"w");
		while(fgets(buf,bsize,infile)!=NULL){
			    location+=strlen(buf);
			    endpoint=strstr(buf,"</item>");
			    if(location>trigger*1024*1024&&endpoint!=NULL){
					printf("trigger %dM\n%s\n",trigger,buf);
					cp=0;
					while( strncmp("</item>",&buf[cp],7)!=0){
						fprintf(outfile,"%c",buf[cp]);
						printf("%c",buf[cp]);
						cp++;
					}
					printf("cut at %s \n",&buf[cp]);
					location=0;
					fprintf(outfile,"%s",tail);
					fclose(outfile);
					num++;
					sprintf(filename,"%s_%d.xml",argv[1],num);
					outfile=fopen(filename,"w");
					fprintf(outfile,"%s",head1);
					fprintf(outfile,"%s",head2);
					fprintf(outfile,"%s",head3);
					fprintf(outfile,"%s",&buf[cp+7]);
					
				}else{
					fprintf(outfile,"%s",buf);
				}
				
		}
		fclose(outfile);
		fclose(infile);
		return 0;
	}
