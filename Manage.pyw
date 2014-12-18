#!python
'''Project manager script for RRR Unity3D project
'''

import Tkinter
import ttk
import os
import codecs


def fix_code(lines, ommit):
    '''Sort lines of imports and usings, etc
    '''
    try:
        soi = 0 # Start of Imports, ommit comments
        while lines[soi][0] in ommit:
            soi = lines.index('', soi)
        eoi = lines.index('', soi) # End of Imports
        return sorted(lines[:eoi]) + lines[eoi:]

    except IndexError:
        return lines

SOURCE_EXTS = {
    'cs': lambda x: fix_code(x, '/ '),
    'py': lambda x: fix_code(x, "#'"),
}


DOWNLOADS = (
    ('Unity 3D 4.5.5+',
        'https://unity3d.com/unity/download'),
    ('Visual Studio Express (optional)',
        'http://www.visualstudio.com/en-us/products/visual-studio-express-vs.aspx'),
    ('Wings 3D (modelling only)',
        'http://www.wings3d.com/?page_id=84'),
)


def code_cleanup():
    '''Cleanup source code
    '''
    yield 'Cleanup...'
    for dir_path, _, file_list in os.walk('.'):
        for file_name in file_list:
            file_path = os.path.join(dir_path, file_name)
            file_ext = file_name.split('.')[-1]
            if file_ext not in SOURCE_EXTS:
                continue

            with open(file_path, 'r') as f:
                original = f.read()

            lines = (line.rstrip().lstrip(codecs.BOM_UTF8).replace('\t', ' '*4)
                 for line in original.split('\n'))
            fix = SOURCE_EXTS[file_ext]

            clean = '\n'.join(fix(list(lines)))
            if clean != original:
                yield 'Fix: ' + file_path
                with open(file_path, 'w') as f:
                    f.write(clean)
            else:
                yield 'Good: ' + file_path
    yield 'Done!'


class ManagerGui(object):
    '''Project manager for RRR
    '''
    def __init__(self):
        self.root = Tkinter.Tk()
        self.root.wm_title(self.__doc__.strip())
        self.root.maxsize(width=600, height=600)

        self.panel = Tkinter.Frame(self.root, width=100, borderwidth=5)
        self.panel.pack(side='left', fill='both')

        ttk.Label(self.panel, text="Download tools:").pack(side='top')
        for tool, link in DOWNLOADS:
            self.download_button(tool, link)

        self.action_button(code_cleanup.__doc__.strip(), code_cleanup())
        ttk.Label(self.panel, text="Actions:").pack(side='bottom')

        self.status = Tkinter.Text(self.root, wrap='word', borderwidth=5)
        self.status.configure(state="disabled")
        self.status.pack(side='right', fill='both')

        self.update_status('Ready!')

    def download_button(self, text, link):
        def browse(button):
            os.startfile(link)
            self.update_status('Open: ' + link)

        b = ttk.Button(self.panel, text=text)
        b.bind("<Button-1>", browse)
        b.pack(side='top', fill='x')

    def action_button(self, text, action):
        def todo(button):
            for message in action:
                self.update_status(message, drop=1)
            self.update_status(drop=1)

        b = ttk.Button(self.panel, text=text)
        b.bind("<Button-1>", todo)
        b.pack(side='bottom', fill='x')

    def update_status(self, message='', drop=2):
        self.status.configure(state="normal")
        self.status.insert(Tkinter.END, message + '\n' * drop)
        self.status.configure(state="disabled")


if __name__ == '__main__':
    ManagerGui().root.mainloop()
