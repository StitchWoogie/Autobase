#include <afxwin.h>
#include <stdio.h>
#include <dos.h>

#include <tools.h>
#include <glib.h>
 
// EF BB BF 가 있는 경우 스킵한다. Byte Order Markers
void SkipUtf8BOM(FILE *in)
{
	int ch;
	ch = fgetc(in);

	if(ch == 0xEF) {
		ch = fgetc(in);
		if(ch == 0xBB) {
			ch = fgetc(in);
			if(ch == 0xBF) {
				return;
			}
		}
	}
	fseek(in, 0, SEEK_SET);
}

// 한글도 잘 됨
void GetProfileStringFromUTF8(const char *section, const char *item, const char *default_value, CString &buf, const char *path)
{
	FILE *in;

	in = fopen(path, "rb");

	if(in == NULL) {
		buf = default_value;
		return;
	}

	SkipUtf8BOM(in);

	CString one_line;
	CString command;
	bool ok_section = false;
	CommaBlockString comma;

	while(1) {
		if(!TextGetOneLineFromUTF8(in, one_line))	break;

		if(!ok_section) {
			if(one_line[0] != '[')	continue;

			comma.Set(one_line);
            comma.SetBlockCode('[');
            comma.Skip();
            comma.SetBlockCode(']');
            comma.GetString(command);
            if (section == command)
            {
                ok_section = true;
            }
		}
		else {
			if (one_line[0] == '[')
            {
				break;
                //ok_section = false;
            }
            else
            {
                comma.Set(one_line);
                comma.SetBlockCode('=');
                comma.GetString(command);
                if (command == item)
                {
                    comma.GetStringTotalRemain(buf);
                    fclose(in);
					return;
                }
            }
		}
	}
	fclose(in);

	buf = default_value;
}